using Microsoft.EntityFrameworkCore;
using SalesOrderSystem.Application.Models;
using SalesOrderSystem.Application.Interfaces;
using SalesOrderSystem.Domain.Entities;
using SalesOrderSystem.Infrastructure.Data;

namespace SalesOrderSystem.Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ApplicationDbContext _context;

        public SalesOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.SalesOrders
                .Include(so => so.Client)
                .Include(so => so.SalesOrderLines)
                .ThenInclude(line => line.Item)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        public async Task<SalesOrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.SalesOrders
                .Include(so => so.Client)
                .Include(so => so.SalesOrderLines)
                .ThenInclude(line => line.Item)
                .FirstOrDefaultAsync(so => so.Id == id);

            return order == null ? null : MapToDto(order);
        }

        public async Task<SalesOrderDto> CreateOrderAsync(CreateSalesOrderDto dto)
        {
            var orderNumber = await GenerateOrderNumberAsync();

            var order = new SalesOrder
            {
                OrderNumber = orderNumber,
                ClientId = dto.ClientId,
                OrderDate = dto.OrderDate,
                DeliveryAddress = dto.DeliveryAddress,
                City = dto.City,
                PostalCode = dto.PostalCode,
                CreatedDate = DateTime.UtcNow
            };

            foreach (var lineDto in dto.Lines)
            {
                var item = await _context.Items.FindAsync(lineDto.ItemId);
                if (item == null) continue;

                var exclAmount = lineDto.Quantity * item.Price;
                var taxAmount = exclAmount * lineDto.TaxRate / 100;
                var inclAmount = exclAmount + taxAmount;

                var line = new SalesOrderLine
                {
                    ItemId = lineDto.ItemId,
                    Note = lineDto.Note,
                    Quantity = lineDto.Quantity,
                    Price = item.Price,
                    TaxRate = lineDto.TaxRate,
                    ExclAmount = exclAmount,
                    TaxAmount = taxAmount,
                    InclAmount = inclAmount
                };

                order.SalesOrderLines.Add(line);
            }

            order.TotalExclAmount = order.SalesOrderLines.Sum(l => l.ExclAmount);
            order.TotalTaxAmount = order.SalesOrderLines.Sum(l => l.TaxAmount);
            order.TotalInclAmount = order.SalesOrderLines.Sum(l => l.InclAmount);

            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();

            return (await GetOrderByIdAsync(order.Id))!;
        }

        public async Task<SalesOrderDto> UpdateOrderAsync(int id, CreateSalesOrderDto dto)
        {
            var order = await _context.SalesOrders
                .Include(so => so.SalesOrderLines)
                .FirstOrDefaultAsync(so => so.Id == id);

            if (order == null)
                throw new Exception("Order not found");

            order.ClientId = dto.ClientId;
            order.OrderDate = dto.OrderDate;
            order.DeliveryAddress = dto.DeliveryAddress;
            order.City = dto.City;
            order.PostalCode = dto.PostalCode;
            order.ModifiedDate = DateTime.UtcNow;

            _context.SalesOrderLines.RemoveRange(order.SalesOrderLines);

            foreach (var lineDto in dto.Lines)
            {
                var item = await _context.Items.FindAsync(lineDto.ItemId);
                if (item == null) continue;

                var exclAmount = lineDto.Quantity * item.Price;
                var taxAmount = exclAmount * lineDto.TaxRate / 100;
                var inclAmount = exclAmount + taxAmount;

                var line = new SalesOrderLine
                {
                    SalesOrderId = order.Id,
                    ItemId = lineDto.ItemId,
                    Note = lineDto.Note,
                    Quantity = lineDto.Quantity,
                    Price = item.Price,
                    TaxRate = lineDto.TaxRate,
                    ExclAmount = exclAmount,
                    TaxAmount = taxAmount,
                    InclAmount = inclAmount
                };

                order.SalesOrderLines.Add(line);
            }

            order.TotalExclAmount = order.SalesOrderLines.Sum(l => l.ExclAmount);
            order.TotalTaxAmount = order.SalesOrderLines.Sum(l => l.TaxAmount);
            order.TotalInclAmount = order.SalesOrderLines.Sum(l => l.InclAmount);

            await _context.SaveChangesAsync();

            return (await GetOrderByIdAsync(order.Id))!;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order == null) return false;

            _context.SalesOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var lastOrder = await _context.SalesOrders
                .OrderByDescending(so => so.Id)
                .FirstOrDefaultAsync();

            var nextNumber = lastOrder == null ? 1 : lastOrder.Id + 1;
            return $"SO{DateTime.Now:yyyyMMdd}{nextNumber:D4}";
        }

        private SalesOrderDto MapToDto(SalesOrder order)
        {
            return new SalesOrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                ClientId = order.ClientId,
                CustomerName = order.Client.CustomerName,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                City = order.City,
                PostalCode = order.PostalCode,
                TotalExclAmount = order.TotalExclAmount,
                TotalTaxAmount = order.TotalTaxAmount,
                TotalInclAmount = order.TotalInclAmount,
                Lines = order.SalesOrderLines.Select(line => new SalesOrderLineDto
                {
                    Id = line.Id,
                    ItemId = line.ItemId,
                    ItemCode = line.Item.ItemCode,
                    Description = line.Item.Description,
                    Note = line.Note,
                    Quantity = line.Quantity,
                    Price = line.Price,
                    TaxRate = line.TaxRate,
                    ExclAmount = line.ExclAmount,
                    TaxAmount = line.TaxAmount,
                    InclAmount = line.InclAmount
                }).ToList()
            };
        }
    }
}