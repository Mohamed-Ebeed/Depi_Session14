using Library;
using Microsoft.EntityFrameworkCore;

using var db = new LibraryContext();
db.Database.EnsureCreated();

var author = new Author { Name = "Naguib Mahfouz", BirthDate = new DateTime(1911, 12, 11) };
var book = new Book { Title = "Palace Walk", ISBN = "9780385264662", Author = author };
var borrower = new Borrower { Name = "Sara", MembershipDate = DateTime.Today };
db.Loans.Add(new Loan { Book = book, Borrower = borrower, LoanDate = DateTime.Today });
db.SaveChanges();

var books = db.Books.Include(b => b.Author).Include(b => b.Borrowers).ToList();
foreach (var b in books)
    Console.WriteLine($"{b.Title} by {b.Author.Name} - borrowed by: {string.Join(", ", b.Borrowers.Select(x => x.Name))}");
