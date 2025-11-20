using Microsoft.EntityFrameworkCore;
using SalesOrderSystem.Domain.Entities;

namespace SalesOrderSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DeliveryAddress).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.TotalExclAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalTaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalInclAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Client)
                    .WithMany(c => c.SalesOrders)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesOrderLine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.ExclAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InclAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.SalesOrder)
                    .WithMany(so => so.SalesOrderLines)
                    .HasForeignKey(e => e.SalesOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Item)
                    .WithMany(i => i.SalesOrderLines)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, CustomerName = "ABC Corporation", Address = "123 Main St", City = "Colombo", PostalCode = "00100", CreatedDate = DateTime.UtcNow },
                new Client { Id = 2, CustomerName = "XYZ Ltd", Address = "456 Galle Road", City = "Colombo", PostalCode = "00300", CreatedDate = DateTime.UtcNow },
                new Client { Id = 3, CustomerName = "Tech Solutions", Address = "789 Kandy Road", City = "Kandy", PostalCode = "20000", CreatedDate = DateTime.UtcNow }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, ItemCode = "ITEM001", Description = "Laptop Computer", Price = 85000.00m, CreatedDate = DateTime.UtcNow },
                new Item { Id = 2, ItemCode = "ITEM002", Description = "Wireless Mouse", Price = 1500.00m, CreatedDate = DateTime.UtcNow },
                new Item { Id = 3, ItemCode = "ITEM003", Description = "Keyboard", Price = 2500.00m, CreatedDate = DateTime.UtcNow },
                new Item { Id = 4, ItemCode = "ITEM004", Description = "Monitor 24 inch", Price = 35000.00m, CreatedDate = DateTime.UtcNow },
                new Item { Id = 5, ItemCode = "ITEM005", Description = "USB Cable", Price = 500.00m, CreatedDate = DateTime.UtcNow }
            );
        }
    }
}