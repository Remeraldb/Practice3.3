// ============================================================
//  Program.cs
// ============================================================
using Microsoft.EntityFrameworkCore;
using NorthwindORM.Data;

var builder = WebApplication.CreateBuilder(args);

// ── EF Core (SQL Server) ─────────────────────────────────────
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Northwind")));

// ── Controllers + Swagger ────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // уникаємо циклічних посилань при серіалізації
        opt.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title   = "Northwind ORM API",
        Version = "v1",
        Description = "ASP.NET Core 8 + EF Core + Northwind (ПР 3.3)"
    });
});

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind ORM API v1");
    c.RoutePrefix = string.Empty; // Swagger на кореневому URL
});

app.UseAuthorization();
app.MapControllers();

app.Run();
