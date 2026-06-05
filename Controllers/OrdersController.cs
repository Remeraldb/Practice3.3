using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly NorthwindContext _db;
    public OrdersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        Ok(await _db.Orders
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new {
                o.OrderId, o.OrderDate, o.ShippedDate, o.Freight, o.ShipCountry, o.ShipCity,
                Customer = o.Customer != null ? o.Customer.CompanyName : null,
                Employee = o.Employee != null ? o.Employee.FirstName + " " + o.Employee.LastName : null
            })
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.ShipViaNavigation)
            .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        order.OrderDate = DateTime.UtcNow;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Order order)
    {
        if (id != order.OrderId) return BadRequest();
        _db.Entry(order).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return NotFound();
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
