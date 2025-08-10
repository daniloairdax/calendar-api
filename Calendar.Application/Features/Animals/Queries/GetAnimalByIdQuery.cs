using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Animals.Models;
using Calendar.Application.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Animals.Queries
{
    public class GetAnimalByIdQuery : IRequest<AnimalDto?>
    {
        public Guid Id { get; }

        public GetAnimalByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAnimalByIdQueryValidator : AbstractValidator<GetAnimalByIdQuery>
    {
        public GetAnimalByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Animal Id is required.");
        }
    }

    public class Handler : IRequestHandler<GetAnimalByIdQuery, AnimalDto?>
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IMapper _mapper;

        public Handler(IAnimalRepository animalRepository, IMapper mapper)
        {
            _animalRepository = animalRepository;
            _mapper = mapper;
        }

        public async Task<AnimalDto?> Handle(GetAnimalByIdQuery request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.Id, cancellationToken);

            if (animal == null)
            {
                throw new NotFoundException("Animal", request.Id);
            }

            return _mapper.Map<AnimalDto>(animal);
        }
    }
}