// ============================================================
//  Migrations/20240102000000_AddEmployeeEmail.cs
//  Міграція 2 — додаємо колонку Email до таблиці Employees
//  (Генерується: dotnet ef migrations add AddEmployeeEmail)
// ============================================================
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthwindORM.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");
        }
    }
}
