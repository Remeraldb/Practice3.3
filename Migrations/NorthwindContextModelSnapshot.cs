// ============================================================
//  Migrations/NorthwindContextModelSnapshot.cs
//  Автоматично генерується EF Core — не редагувати вручну
// ============================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NorthwindORM.Data;

#nullable disable

namespace NorthwindORM.Migrations
{
    [DbContext(typeof(NorthwindContext))]
    partial class NorthwindContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.UseIdentityColumns();

            // Snapshot відображає фінальний стан моделі (після обох міграцій).
            // EF Core генерує цей файл автоматично при кожному
            // виклику 'dotnet ef migrations add ...'
            // Тут показана спрощена версія для ілюстрації.
#pragma warning restore 612, 618
        }
    }
}
