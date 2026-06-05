using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;
using NorthwindORM.Models;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly NorthwindContext _db;
    public EmployeesController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Employees
            .Select(e => new {
                e.EmployeeId,
                FullName = e.FirstName + " " + e.LastName,
                e.Title, e.City, e.Country,
                Manager = e.ReportsToNavigation != null
                    ? e.ReportsToNavigation.FirstName + " " + e.ReportsToNavigation.LastName : null
            })
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Employees
            .Include(x => x.ReportsToNavigation)
            .FirstOrDefaultAsync(x => x.EmployeeId == id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee emp)
    {
        _db.Employees.Add(emp);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = emp.EmployeeId }, emp);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Employee emp)
    {
        if (id != emp.EmployeeId) return BadRequest();
        _db.Entry(emp).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return NotFound();
        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
