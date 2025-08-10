using AutoMapper;
using Calendar.Application.Features.Animals.Models;
using Calendar.Infrastructure.Persistence;
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
        private readonly CalendarDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(CalendarDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AnimalDto?> Handle(GetAnimalByIdQuery request, CancellationToken cancellationToken)
        {
            var animal = await _dbContext.Animals.FindAsync(new object[] { request.Id }, cancellationToken);
            if (animal == null)
            {
                throw new NotFoundException("Animal", request.Id);
            }

            return _mapper.Map<AnimalDto>(animal);
        }
    }
}