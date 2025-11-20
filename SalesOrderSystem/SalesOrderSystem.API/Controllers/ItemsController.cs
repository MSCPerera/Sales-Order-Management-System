using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderSystem.API.Models;
using SalesOrderSystem.Infrastructure.Data;

namespace SalesOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _context.Items
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    Description = i.Description,
                    Price = i.Price
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return NotFound();

            var dto = new ItemDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Description = item.Description,
                Price = item.Price
            };

            return Ok(dto);
        }
    }
}