# ПР 3.3 — Northwind ORM API
### ASP.NET Core 8 · Entity Framework Core 8 · Swagger · SQL Server

---

## Структура проєкту

```
NorthwindORM/
├── Controllers/
│   ├── QueryController.cs               ← LINQ-запити 3–6 (ЛР 2)
│   ├── ProductsController.cs            ← CRUD Products
│   ├── OrdersController.cs              ← CRUD Orders
│   ├── EmployeesController.cs           ← CRUD Employees + PATCH email
│   ├── CustomersController.cs           ← CRUD Customers
│   └── CategoriesAndSuppliersController.cs
├── Data/
│   └── NorthwindContext.cs              ← DbContext (Database First)
├── Models/
│   └── NorthwindModels.cs               ← Усі entity-класи
├── Migrations/
│   ├── 20240101000000_InitialCreate.cs  ← Міграція 1: початкова схема
│   ├── 20240102000000_AddEmployeeEmail.cs ← Міграція 2: +Email до Employees
│   └── NorthwindContextModelSnapshot.cs
├── appsettings.json
├── Program.cs
└── NorthwindORM.csproj
```

---

## Налаштування та запуск

### 1. Вимоги
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server або SQL Server Express / LocalDB
- [SQL Server Management Studio](https://aka.ms/ssmsfullsetup) (опційно)

### 2. Відновити БД з SQL-скрипту
Відкрийте SSMS або `sqlcmd` та виконайте скрипт `LabWork2.sql`:
```sql
-- У SSMS: File → Open → LabWork2.sql → Execute (F5)
-- або через sqlcmd:
sqlcmd -S "(localdb)\mssqllocaldb" -d master -i LabWork2.sql
```

### 3. Рядок підключення
У файлі `appsettings.json` відредагуйте рядок під вашу інсталяцію:
```json
"ConnectionStrings": {
  "Northwind": "Server=(localdb)\\mssqllocaldb;Database=Northwind;Trusted_Connection=True;"
}
```

| Варіант           | Server                          |
|-------------------|---------------------------------|
| LocalDB (default) | `(localdb)\\mssqllocaldb`       |
| SQL Server Express| `.\SQLEXPRESS`                  |
| SQL Server Full   | `.` або `localhost`             |

### 4. Встановити EF Core Tools (один раз)
```bash
dotnet tool install --global dotnet-ef
```

### 5. Застосувати міграції
```bash
cd NorthwindORM

# Застосувати обидві міграції (InitialCreate + AddEmployeeEmail)
dotnet ef database update

# Переглянути список міграцій:
dotnet ef migrations list
```

### 6. Запустити проєкт
```bash
dotnet run
```
Swagger UI відкриється автоматично на: **http://localhost:5000**

---

## Міграції (завдання п. 3)

### Міграція 1 — `InitialCreate`
Створює повну схему Northwind: усі таблиці, первинні та зовнішні ключі,
індекси.

```bash
# Команда, якою було згенеровано (Database First / Scaffolding):
dotnet ef migrations add InitialCreate
```

### Міграція 2 — `AddEmployeeEmail`
Додає колонку `Email` (`nvarchar(100)`, nullable) до таблиці `Employees`.

```bash
# Команда генерації:
dotnet ef migrations add AddEmployeeEmail

# Застосувати лише до цієї міграції:
dotnet ef database update AddEmployeeEmail

# Відкотити:
dotnet ef database update InitialCreate
```

---

## Ендпоінти — LINQ-запити ЛР 2 (QueryController)

| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/query/query3-suppliers-product-count` | **Запит 3** — постачальники та кількість їх продуктів |
| GET | `/api/query/query4-orders-over-5000` | **Запит 4** — замовлення із загальною вартістю > 5000 |
| GET | `/api/query/query5-top5-products` | **Запит 5** — топ-5 продуктів за кількістю проданих одиниць |
| GET | `/api/query/query6-employees-revenue` | **Запит 6** — співробітники, кількість замовлень та виручка |

---

## CRUD Ендпоінти

### Products
| Метод  | URL                    | Опис               |
|--------|------------------------|--------------------|
| GET    | `/api/products`        | Всі продукти       |
| GET    | `/api/products/{id}`   | Продукт за ID      |
| POST   | `/api/products`        | Створити продукт   |
| PUT    | `/api/products/{id}`   | Оновити продукт    |
| DELETE | `/api/products/{id}`   | Видалити продукт   |

### Orders
| Метод  | URL                              | Опис                           |
|--------|----------------------------------|--------------------------------|
| GET    | `/api/orders?page=1&pageSize=20` | Замовлення з пагінацією        |
| GET    | `/api/orders/{id}`               | Замовлення з деталями          |
| POST   | `/api/orders`                    | Створити замовлення            |
| PUT    | `/api/orders/{id}`               | Оновити замовлення             |
| DELETE | `/api/orders/{id}`               | Видалити замовлення            |

### Employees
| Метод  | URL                           | Опис                          |
|--------|-------------------------------|-------------------------------|
| GET    | `/api/employees`              | Всі співробітники             |
| GET    | `/api/employees/{id}`         | Співробітник за ID            |
| POST   | `/api/employees`              | Створити співробітника        |
| PUT    | `/api/employees/{id}`         | Оновити співробітника         |
| DELETE | `/api/employees/{id}`         | Видалити співробітника        |
| PATCH  | `/api/employees/{id}/email`   | Оновити Email (нова колонка)  |

### Customers
| Метод  | URL                              | Опис                       |
|--------|----------------------------------|----------------------------|
| GET    | `/api/customers?country=Germany` | Фільтр за країною          |
| GET    | `/api/customers/{id}`            | Клієнт за ID               |
| POST   | `/api/customers`                 | Створити клієнта           |
| PUT    | `/api/customers/{id}`            | Оновити клієнта            |
| DELETE | `/api/customers/{id}`            | Видалити клієнта           |

### Categories / Suppliers
Аналогічні CRUD-ендпоінти: `/api/categories`, `/api/suppliers`

---

## LINQ-запити — пояснення

### Запит 3 (Постачальники + кількість продуктів)
```csharp
_db.Suppliers
    .GroupJoin(_db.Products,
        s => s.SupplierID, p => p.SupplierID,
        (supplier, products) => new {
            supplier.CompanyName,
            ProductCount = products.Count()
        })
    .OrderByDescending(x => x.ProductCount)
```

### Запит 4 (Замовлення > 5000)
```csharp
_db.Orders
    .Select(o => new {
        o.OrderID, o.OrderDate,
        TotalAmount = o.OrderDetails
            .Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
    })
    .Where(x => x.TotalAmount > 5000)
    .OrderByDescending(x => x.TotalAmount)
```

### Запит 5 (Топ-5 продуктів)
```csharp
_db.Products
    .Select(p => new {
        p.ProductName,
        CategoryName = p.Category.CategoryName,
        TotalUnitsSold = p.OrderDetails.Sum(od => (int)od.Quantity)
    })
    .OrderByDescending(x => x.TotalUnitsSold)
    .Take(5)
```

### Запит 6 (Співробітники + виручка за 2 роки)
```csharp
_db.Employees
    .Select(e => new {
        FullName = e.FirstName + " " + e.LastName,
        OrderCount = e.Orders.Count(o => o.OrderDate >= cutoff),
        TotalRevenue = e.Orders
            .Where(o => o.OrderDate >= cutoff)
            .SelectMany(o => o.OrderDetails)
            .Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
    })
    .OrderByDescending(x => x.TotalRevenue)
```

---

## Технологічний стек

| Компонент | Версія |
|-----------|--------|
| .NET | 8.0 |
| ASP.NET Core Web API | 8.0 |
| Entity Framework Core | 8.0 |
| EF Core SQL Server Provider | 8.0 |
| Swashbuckle (Swagger) | 6.5 |
| БД | Microsoft SQL Server / LocalDB |
