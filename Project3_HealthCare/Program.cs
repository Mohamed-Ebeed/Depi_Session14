using HealthCare;
using Microsoft.EntityFrameworkCore;

using var db = new HealthCareContext();
db.Database.EnsureCreated();

var patient = new Patient { Name = "Omar", DateOfBirth = new DateTime(1995, 5, 20) };
var doctor = new Doctor { Name = "Dr. Hana", Specialization = "Cardiology" };
db.Appointments.Add(new Appointment { Patient = patient, Doctor = doctor, AppointmentDate = DateTime.Now.AddDays(3) });
db.SaveChanges();

var patients = db.Patients.Include(p => p.Doctors).ToList();
foreach (var p in patients)
    Console.WriteLine($"{p.Name} sees: {string.Join(", ", p.Doctors.Select(d => d.Name + " (" + d.Specialization + ")"))}");
