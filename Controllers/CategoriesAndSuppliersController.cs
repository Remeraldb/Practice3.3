using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly NorthwindContext _db;
    public CategoriesController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Categories
            .Select(c => new {
                c.CategoryId, c.CategoryName, c.Description,
                ProductCount = c.Products.Count()
            })
            .OrderBy(c => c.CategoryName)
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _db.Categories
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.CategoryId == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.CategoryId) return BadRequest();
        _db.Entry(category).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Categories.FindAsync(id);
        if (c is null) return NotFound();
        _db.Categories.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
