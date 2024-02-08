
using Amazon.DynamoDBv2.DataModel;

namespace dotnet_aws_dynamodb.Models
{
    [DynamoDBTable("Sale")]
    public abstract class Sale
    {
        [DynamoDBHashKey]
        public string InvoiceNo { get; set; }

        [DynamoDBRangeKey]
        public string SK { get; set; }
    }

    public class SaleHeader : Sale
    {
        public string Date { get; set; }
        public int TotalItem { get; set; }
        public int TotalAmount { get; set; }

    }

    public class SaleItem : Sale
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
        public int Amount { get; set; }
    }
}
