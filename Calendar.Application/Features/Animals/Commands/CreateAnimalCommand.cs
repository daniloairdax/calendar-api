using AutoMapper;
using Calendar.Application.Features.Animals.Models;
using Calendar.Application.Interfaces;
using Calendar.Domain.Models;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Animals.Commands
{
    public class CreateAnimalCommand : IRequest<AnimalDto>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
    }

    public class CreateAnimalCommandValidator : AbstractValidator<CreateAnimalCommand>
    {
        public CreateAnimalCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Animal name is required.")
                .MaximumLength(100).WithMessage("Animal name must be less than 100 characters.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now).WithMessage("Birth date must be in the past.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.");

            RuleFor(x => x.OwnerName)
                .NotEmpty().WithMessage("Owner name is required.")
                .MaximumLength(100).WithMessage("Owner name must be less than 100 characters.");

            RuleFor(x => x.OwnerEmail)
                .NotEmpty().WithMessage("Owner email is required.")
                .EmailAddress().WithMessage("Owner email must be a valid email address.");
        }
    }

    public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand, AnimalDto>
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IMapper _mapper;

        public CreateAnimalCommandHandler(IAnimalRepository animalRepository, IMapper mapper)
        {
            _animalRepository = animalRepository;
            _mapper = mapper;
        }

        public async Task<AnimalDto> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = new Animal
            {
                Name = request.Name,
                BirthDate = request.BirthDate,
                OwnerId = request.OwnerId,
                OwnerName = request.OwnerName,
                OwnerEmail = request.OwnerEmail
            };

            await _animalRepository.AddAsync(animal, cancellationToken);

            return _mapper.Map<AnimalDto>(animal);
        }
    }
}