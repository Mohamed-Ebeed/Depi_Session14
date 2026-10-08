using Microsoft.EntityFrameworkCore;

namespace HealthCare;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}

public class Appointment
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}

public class HealthCareContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer(@"Server=.;Database=HealthCareDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Patient * --- * Doctor through Appointment
        mb.Entity<Patient>()
          .HasMany(p => p.Doctors).WithMany(d => d.Patients)
          .UsingEntity<Appointment>(
              j => j.HasOne(a => a.Doctor).WithMany(d => d.Appointments).HasForeignKey(a => a.DoctorId),
              j => j.HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId),
              j => j.HasKey(a => new { a.PatientId, a.DoctorId, a.AppointmentDate }));
    }
}
