using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly NorthwindContext _db;
    public ProductsController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Products
            .Select(p => new {
                p.ProductId, p.ProductName, p.UnitPrice, p.UnitsInStock, p.Discontinued,
                Category = p.Category != null ? p.Category.CategoryName : null,
                Supplier = p.Supplier != null ? p.Supplier.CompanyName : null
            })
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .FirstOrDefaultAsync(x => x.ProductId == id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.ProductId) return BadRequest();
        _db.Entry(product).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p is null) return NotFound();
        _db.Products.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
