using Calendar.Application.Features.Animals.Queries;
using Calendar.Domain.Models;
using Calendar.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Animals.Commands
{
    public class DeleteAnimalCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteAnimalCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteAnimalCommandValidator : AbstractValidator<DeleteAnimalCommand>
    {
        public DeleteAnimalCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Animal Id is required.");
        }
    }

    public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, bool>
    {
        private readonly CalendarDbContext _dbContext;
        private readonly IMediator _mediator;

        public DeleteAnimalCommandHandler(CalendarDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DeleteAnimalCommand request, CancellationToken cancellationToken)
        {
            // Check if AnimalId exists
            var animal = await _dbContext.Animals.FindAsync(new object[] { request.Id }, cancellationToken);
            if (animal == null)
            {
                throw new NotFoundException("Animal", request.Id);
            }

            _dbContext.Animals.Remove(animal);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}