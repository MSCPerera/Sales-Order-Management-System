namespace SalesOrderSystem.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public ICollection<SalesOrderLine> SalesOrderLines { get; set; } = new List<SalesOrderLine>();
    }
}