using Microsoft.AspNetCore.Mvc;
using SalesOrderSystem.Application.Models;
using SalesOrderSystem.Application.Interfaces;

namespace SalesOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrdersController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderDto>>> GetOrders()
        {
            var orders = await _salesOrderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderDto>> GetOrder(int id)
        {
            var order = await _salesOrderService.GetOrderByIdAsync(id);
            
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<SalesOrderDto>> CreateOrder(CreateSalesOrderDto dto)
        {
            try
            {
                var order = await _salesOrderService.CreateOrderAsync(dto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SalesOrderDto>> UpdateOrder(int id, CreateSalesOrderDto dto)
        {
            try
            {
                var order = await _salesOrderService.UpdateOrderAsync(id, dto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _salesOrderService.DeleteOrderAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}