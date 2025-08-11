using Calendar.Application.Interfaces;
using Calendar.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Infrastructure.Persistence
{
    [ExcludeFromCodeCoverage]
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ICalendarDbContext _context;

        public AppointmentRepository(ICalendarDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments
                .Include(a => a.Animal)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetByVetAndDateRangeAsync(Guid vetId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments
                .Include(a => a.Animal)
                .Where(a => a.VeterinarianId == vetId && a.StartTime >= startDate && a.EndTime <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
        {
            await _context.Appointments.AddAsync(appointment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var appointment = await GetByIdAsync(id, cancellationToken);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}