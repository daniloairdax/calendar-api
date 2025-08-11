using Calendar.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Calendar.Infrastructure.Persistence
{
    /// <summary>
    /// Seeds the database with initial data for testing purposes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DbSeeder
    {
        public static void Seed(CalendarDbContext context)
        {
            if (!context.Animals.Any())
            {
                context.Animals.AddRange(
                    new Animal
                    {
                        Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                        Name = "Dog",
                        BirthDate = DateTime.Now.AddYears(-3),
                        OwnerId = Guid.NewGuid(),
                        OwnerName = "Dog Owner",
                        OwnerEmail = "dogowner@example.com"
                    },
                    new Animal
                    {
                        Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d477"),
                        Name = "Cat",
                        BirthDate = DateTime.Now.AddYears(-2),
                        OwnerId = Guid.NewGuid(),
                        OwnerName = "Cat Owner",
                        OwnerEmail = "catowner@example.com"
                    },
                    new Animal
                    {
                        Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d476"),
                        Name = "Rabbit",
                        BirthDate = DateTime.Now.AddYears(-1),
                        OwnerId = Guid.NewGuid(),
                        OwnerName = "Rabbit Owner",
                        OwnerEmail = "rabbitsowner@example.com"
                    }
                );
                context.SaveChanges();
            }

            if (!context.Appointments.Any())
            {
                var dog = context.Animals.FirstOrDefault(a => a.Name == "Dog");
                var cat = context.Animals.FirstOrDefault(a => a.Name == "Cat");
                var vetId = Guid.NewGuid();
                if (dog != null)
                {
                    context.Appointments.Add(new Appointment
                    {
                        Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                        AnimalId = dog.Id,
                        CustomerId = dog.OwnerId,
                        VeterinarianId = vetId,
                        StartTime = DateTime.Now.AddDays(1),
                        EndTime = DateTime.Now.AddDays(1).AddHours(1),
                        Status = Domain.Enums.AppointmentStatus.Scheduled,
                        Notes = "Vet appointment"
                    });
                }
                if (cat != null)
                {
                    context.Appointments.Add(new Appointment
                    {
                        Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d480"),
                        AnimalId = cat.Id,
                        CustomerId = cat.OwnerId,
                        VeterinarianId = vetId,
                        StartTime = DateTime.Now.AddDays(2),
                        EndTime = DateTime.Now.AddDays(2).AddHours(1),
                        Status = Domain.Enums.AppointmentStatus.Scheduled,
                        Notes = "Follow-up check"
                    });
                }
                context.SaveChanges();
            }
        }
    }
}
