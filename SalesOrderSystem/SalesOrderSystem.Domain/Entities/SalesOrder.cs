namespace SalesOrderSystem.Domain.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public decimal TotalExclAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalInclAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        
        public Client Client { get; set; } = null!;
        public ICollection<SalesOrderLine> SalesOrderLines { get; set; } = new List<SalesOrderLine>();
    }
}