using Calendar.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Interfaces
{
    public interface ICalendarDbContext
    {
        DbSet<Animal> Animals { get; set; }
        DbSet<Appointment> Appointments { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
