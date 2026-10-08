using Microsoft.EntityFrameworkCore;

namespace ECommerce;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class ECommerceContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer(@"Server=.;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

        // Category 1 --- * Product
        mb.Entity<Category>()
          .HasMany(c => c.Products).WithOne(p => p.Category)
          .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

        // Customer 1 --- * Order
        mb.Entity<Customer>()
          .HasMany(c => c.Orders).WithOne(o => o.Customer)
          .HasForeignKey(o => o.CustomerId);

        // Order * --- * Product through OrderDetail
        mb.Entity<Order>()
          .HasMany(o => o.Products).WithMany(p => p.Orders)
          .UsingEntity<OrderDetail>(
              j => j.HasOne(od => od.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductId),
              j => j.HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderId),
              j => j.HasKey(od => new { od.OrderId, od.ProductId }));
    }
}
