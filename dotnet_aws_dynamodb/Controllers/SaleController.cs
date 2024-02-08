using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using dotnet_aws_dynamodb.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_aws_dynamodb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly IDynamoDBContext _dynamoDBContext;

        public SaleController(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale()
        {
            string invoiceNo = "INV003";

            var saleItems = new List<SaleItem>();
            
            var p001 =new SaleItem();
            p001.InvoiceNo = invoiceNo;
            p001.ProductCode="P001";
            p001.ProductName= "IPHONE14";
            p001.SK = $"{invoiceNo}#{p001.ProductCode}";
            p001.Price =10000;
            p001.Qty=1;
            p001.Amount=p001.Price*p001.Qty;

            var p002 = new SaleItem();
            p002.InvoiceNo = invoiceNo;
            p002.ProductCode = "P002";
            p002.ProductName = "IPHONE15";
            p002.SK = $"{invoiceNo}#{p002.ProductCode}";
            p002.Price = 20000;
            p002.Qty = 1;
            p002.Amount = p002.Price * p002.Qty;

            var p003 = new SaleItem();
            p003.InvoiceNo = invoiceNo;
            p003.ProductCode = "P003";
            p003.ProductName = "IPHONE12";
            p003.SK = $"{invoiceNo}#{p003.ProductCode}";
            p003.Price = 15000;
            p003.Qty = 1;
            p003.Amount = p003.Price * p003.Qty;

            saleItems.Add(p001);
            saleItems.Add(p002);
            saleItems.Add(p003);


            var saleHeader =new SaleHeader();
            saleHeader.InvoiceNo = invoiceNo;
            saleHeader.SK = invoiceNo;


            saleHeader.Date = DateTime.Now.ToShortDateString();
            saleHeader.TotalItem = saleItems.Count;
            saleHeader.TotalAmount = saleItems.Sum(x => x.Amount);

            await _dynamoDBContext.SaveAsync(saleHeader);


            foreach (var item in saleItems)
            {
                await _dynamoDBContext.SaveAsync(item);
            }
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSaleByInvoiceNo(string invoiceNo)
        {
            var saleHeader = await _dynamoDBContext.LoadAsync<SaleHeader>(invoiceNo,invoiceNo);

            List<ScanCondition> scs = new List<ScanCondition>();
            var sc1 = new ScanCondition("InvoiceNo", ScanOperator.BeginsWith, invoiceNo);
            var sc2 = new ScanCondition("SK", ScanOperator.NotEqual, invoiceNo);
            scs.Add(sc1);
            scs.Add(sc2);

            AsyncSearch<SaleItem> response = _dynamoDBContext.ScanAsync<SaleItem>(scs);

            IEnumerable<SaleItem> saleItem = await response.GetRemainingAsync();

            return Ok(new { Header = saleHeader, Items = saleItem });
        }

        [HttpGet("GetAllSale")]
        public async Task<IActionResult> GetAllSale()
        {
            List<ScanCondition> scs = new List<ScanCondition>();
            var sc1 = new ScanCondition("TotalItem", ScanOperator.IsNotNull);
            scs.Add(sc1 );

            AsyncSearch<SaleHeader> response = _dynamoDBContext.ScanAsync<SaleHeader>(scs);

            IEnumerable<SaleHeader> sales = await response.GetRemainingAsync();


            List<ScanCondition> scsItem = new List<ScanCondition>();
            var scsItem1 = new ScanCondition("SK", ScanOperator.Contains,"#P");
            scsItem.Add(scsItem1);

            AsyncSearch<SaleItem> responseItem = _dynamoDBContext.ScanAsync<SaleItem>(scsItem);

            IEnumerable<SaleItem> saleItems = await responseItem.GetRemainingAsync();


            return Ok(new { Header = sales, Items = saleItems });

        }
    }
}
