namespace SalesOrderSystem.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}