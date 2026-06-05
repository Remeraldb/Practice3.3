using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly NorthwindContext _db;
    public CustomersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? country = null)
    {
        var q = _db.Customers.AsQueryable();
        if (!string.IsNullOrEmpty(country))
            q = q.Where(c => c.Country == country);
        return Ok(await q.OrderBy(c => c.CompanyName).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var c = await _db.Customers
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.CustomerId == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();
        _db.Entry(customer).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var c = await _db.Customers.FindAsync(id);
        if (c is null) return NotFound();
        _db.Customers.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
