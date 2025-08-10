using Calendar.Application.Interfaces;
using Calendar.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Infrastructure.Persistence
{
    [ExcludeFromCodeCoverage]
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ICalendarDbContext _context;

        public AnimalRepository(ICalendarDbContext context)
        {
            _context = context;
        }

        public async Task<Animal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Animals.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Animal>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Animals.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Animal animal, CancellationToken cancellationToken = default)
        {
            await _context.Animals.AddAsync(animal, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Animal animal, CancellationToken cancellationToken = default)
        {
            _context.Animals.Update(animal);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var animal = await GetByIdAsync(id, cancellationToken);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}