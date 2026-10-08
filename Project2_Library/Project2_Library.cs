using Microsoft.EntityFrameworkCore;

namespace Library;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Borrower> Borrowers { get; set; } = new List<Borrower>();
}

public class Borrower
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime MembershipDate { get; set; }
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Loan
{
    public int BookId { get; set; }
    public int BorrowerId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public Book Book { get; set; } = null!;
    public Borrower Borrower { get; set; } = null!;
}

public class LibraryContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer(@"Server=.;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Book>().HasIndex(b => b.ISBN).IsUnique();

        // Author 1 --- * Book
        mb.Entity<Author>()
          .HasMany(a => a.Books).WithOne(b => b.Author)
          .HasForeignKey(b => b.AuthorId);

        // Book * --- * Borrower through Loan
        mb.Entity<Book>()
          .HasMany(b => b.Borrowers).WithMany(r => r.Books)
          .UsingEntity<Loan>(
              j => j.HasOne(l => l.Borrower).WithMany(r => r.Loans).HasForeignKey(l => l.BorrowerId),
              j => j.HasOne(l => l.Book).WithMany(b => b.Loans).HasForeignKey(l => l.BookId),
              j => j.HasKey(l => new { l.BookId, l.BorrowerId, l.LoanDate }));
    }
}
