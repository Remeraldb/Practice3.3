// ============================================================
//  Controllers/SuppliersController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SuppliersController : ControllerBase
{
    private readonly NorthwindContext _db;
    public SuppliersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Suppliers.OrderBy(s => s.CompanyName).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Suppliers
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.SupplierId == id);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, supplier);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Supplier supplier)
    {
        if (id != supplier.SupplierId) return BadRequest();
        _db.Entry(supplier).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _db.Suppliers.FindAsync(id);
        if (s is null) return NotFound();
        _db.Suppliers.Remove(s);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
