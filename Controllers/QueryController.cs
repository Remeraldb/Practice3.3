using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;

namespace NorthwindORM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class QueryController : ControllerBase
{
    private readonly NorthwindContext _db;
    public QueryController(NorthwindContext db) => _db = db;

    // Запит 3 — Кількість замовлень для кожного клієнта
    [HttpGet("query3-orders-per-customer")]
    public async Task<IActionResult> Query3()
    {
        var result = await _db.Customers
            .GroupJoin(
                _db.Orders,
                c => c.CustomerId,
                o => o.CustomerId,
                (customer, orders) => new
                {
                    customer.CustomerId,
                    customer.CompanyName,
                    customer.Country,
                    OrderCount = orders.Count()
                })
            .OrderByDescending(x => x.OrderCount)
            .ToListAsync();
        return Ok(result);
    }

    // Запит 4 — Звіт замовлень 1996 року з клієнтами (JOIN, 152 рядки)
    [HttpGet("query4-orders-1996")]
    public async Task<IActionResult> Query4()
    {
        var result = await _db.Orders
            .Join(
                _db.Customers,
                o => o.CustomerId,
                c => c.CustomerId,
                (order, customer) => new { order, customer })
            .Where(x => x.order.OrderDate.HasValue && x.order.OrderDate.Value.Year == 1996)
            .OrderBy(x => x.order.OrderDate)
            .Select(x => new
            {
                x.order.OrderId,
                x.order.OrderDate,
                x.order.ShipCountry,
                x.customer.CompanyName,
                x.customer.ContactName,
                x.customer.Country
            })
            .ToListAsync();
        return Ok(result);
    }

    // Запит 5 — 10 найпопулярніших продуктів (найбільше — Côte de Blaye)
    [HttpGet("query5-top10-products")]
    public async Task<IActionResult> Query5()
    {
        var result = await _db.Products
            .Join(
                _db.OrderDetails,
                p => p.ProductId,
                od => od.ProductId,
                (product, od) => new { product, od })
            .GroupBy(x => new { x.product.ProductId, x.product.ProductName })
            .Select(g => new
            {
                g.Key.ProductName,
                TotalQty     = g.Sum(x => (int)x.od.Quantity),
                TotalRevenue = g.Sum(x => x.od.UnitPrice * x.od.Quantity * (decimal)(1 - x.od.Discount))
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(10)
            .ToListAsync();
        return Ok(result);
    }

    // Запит 6 — Продукти відсутні на складі + назва категорії (підзапит, 5 рядків)
    [HttpGet("query6-out-of-stock")]
    public async Task<IActionResult> Query6()
    {
        var result = await _db.Products
            .Where(p => p.UnitsInStock == 0)
            .Select(p => new
            {
                p.ProductName,
                CategoryName = _db.Categories
                    .Where(c => c.CategoryId == p.CategoryId)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault(),
                p.UnitsInStock,
                p.Discontinued
            })
            .OrderBy(x => x.CategoryName)
            .ToListAsync();
        return Ok(result);
    }
}
