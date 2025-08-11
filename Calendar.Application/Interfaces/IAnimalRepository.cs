using Calendar.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calendar.Application.Interfaces
{
    public interface IAnimalRepository
    {
        Task<Animal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Animal>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Animal animal, CancellationToken cancellationToken = default);
        Task UpdateAsync(Animal animal, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}