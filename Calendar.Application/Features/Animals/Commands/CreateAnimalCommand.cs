using Calendar.Application.Features.Animals.Models;
using Calendar.Domain.Models;
using Calendar.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using AutoMapper;
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
        private readonly CalendarDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateAnimalCommandHandler(CalendarDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
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

            _dbContext.Animals.Add(animal);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AnimalDto>(animal);
        }
    }
}