using ECommerce;
using Microsoft.EntityFrameworkCore;

using var db = new ECommerceContext();
db.Database.EnsureCreated();

var category = new Category { Name = "Laptops" };
var product = new Product { Name = "ThinkPad", Price = 1200m, Category = category };
var customer = new Customer { Name = "Ali", Email = "ali@mail.com" };
var order = new Order { OrderDate = DateTime.Now, Customer = customer };
order.OrderDetails.Add(new OrderDetail { Product = product, Quantity = 2 });
db.Add(order);
db.SaveChanges();

var orders = db.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
    .ToList();

foreach (var o in orders)
    foreach (var d in o.OrderDetails)
        Console.WriteLine($"{o.Customer.Name} ordered {d.Quantity} x {d.Product.Name}");
