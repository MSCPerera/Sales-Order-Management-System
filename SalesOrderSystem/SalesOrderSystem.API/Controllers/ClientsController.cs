using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderSystem.API.Models;
using SalesOrderSystem.Infrastructure.Data;

namespace SalesOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _context.Clients
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    Address = c.Address,
                    City = c.City,
                    PostalCode = c.PostalCode
                })
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            var dto = new ClientDto
            {
                Id = client.Id,
                CustomerName = client.CustomerName,
                Address = client.Address,
                City = client.City,
                PostalCode = client.PostalCode
            };

            return Ok(dto);
        }
    }
}