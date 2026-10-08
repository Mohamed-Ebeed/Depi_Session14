# Navigation Properties in EF Core: Let Your Objects Talk, Not Your SQL Joins

When I started with Entity Framework Core, I wrote joins by hand for everything. Then I discovered **navigation properties**, and my queries became shorter, safer, and far more readable.

## What is a Navigation Property?
A navigation property is a property on an entity that points to **another entity** (or a collection of them). It lets you move across relationships in C# instead of writing JOINs in SQL.

```csharp
public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }          // Foreign Key
    public Category Category { get; set; }       // Reference navigation
}

public class Category
{
    public int Id { get; set; }
    public ICollection<Product> Products { get; set; }  // Collection navigation
}
```

## Two Types
- **Reference navigation**: points to a single entity (`product.Category`).
- **Collection navigation**: points to many entities (`category.Products`).

## Relationships You Can Model
1. **One-to-Many**: `Category` → `Product`, `Customer` → `Order`. The "many" side holds the foreign key.
2. **Many-to-Many**: `Order` ↔ `Product` through `OrderDetail`. EF Core supports *skip navigations* (`order.Products`) while the join entity (`OrderDetail`) can still carry extra data like `Quantity`.
3. **One-to-One**: each side holds a reference navigation.

## Querying with Navigation Properties
```csharp
var orders = db.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderDetails)
        .ThenInclude(d => d.Product)
    .ToList();
```
No manual JOINs, and EF Core generates the SQL for you.

## Fluent API Example (Many-to-Many with payload)
```csharp
modelBuilder.Entity<Order>()
    .HasMany(o => o.Products).WithMany(p => p.Orders)
    .UsingEntity<OrderDetail>(
        j => j.HasOne(od => od.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductId),
        j => j.HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderId),
        j => j.HasKey(od => new { od.OrderId, od.ProductId }));
```

## Tips
- Always define the **foreign key property** explicitly (`CategoryId`), as it avoids shadow properties.
- Initialize collection navigations (`= new List<T>()`) to avoid null reference errors.
- Navigation properties are **not loaded automatically**. Use `Include`, explicit loading, or lazy loading, and be careful about N+1 queries.
- Use `OnDelete(DeleteBehavior.Restrict)` where cascade deletes would be dangerous.

## Conclusion
Navigation properties turn your database schema into a clean object graph. Master them and you write less code, make fewer mistakes, and keep your domain model expressive.

I applied this in three mini projects: an E-commerce system, a Library system, and a Health Care system.

#EFCore #dotnet #CSharp #EntityFramework #BackendDevelopment #SoftwareEngineering
