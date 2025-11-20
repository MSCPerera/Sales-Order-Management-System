namespace SalesOrderSystem.API.Models
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public decimal TotalExclAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalInclAmount { get; set; }
        public List<SalesOrderLineDto> Lines { get; set; } = new();
    }

    public class CreateSalesOrderDto
    {
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public List<CreateSalesOrderLineDto> Lines { get; set; } = new();
    }

    public class SalesOrderLineDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ExclAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal InclAmount { get; set; }
    }

    public class CreateSalesOrderLineDto
    {
        public int ItemId { get; set; }
        public string Note { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TaxRate { get; set; }
    }
}