using Calendar.Application.Exceptions;
using Calendar.Application.Interfaces;
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
        private readonly IAnimalRepository _animalRepository;

        public DeleteAnimalCommandHandler(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<bool> Handle(DeleteAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.Id, cancellationToken);

            if (animal == null)
            {
                throw new NotFoundException("Animal", request.Id);
            }

            await _animalRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
    }
}