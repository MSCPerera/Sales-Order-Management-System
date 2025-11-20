using SalesOrderSystem.API.Models;

namespace SalesOrderSystem.Application.Interfaces
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<SalesOrderDto>> GetAllOrdersAsync();
        Task<SalesOrderDto?> GetOrderByIdAsync(int id);
        Task<SalesOrderDto> CreateOrderAsync(CreateSalesOrderDto dto);
        Task<SalesOrderDto> UpdateOrderAsync(int id, CreateSalesOrderDto dto);
        Task<bool> DeleteOrderAsync(int id);
    }
}