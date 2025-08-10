using AutoMapper;
using Calendar.Application.Features.Animals.Queries;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Domain.Enums;
using Calendar.Domain.Models;
using Calendar.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Appointments.Commands
{
    public class CreateAppointmentCommand : IRequest<AppointmentDto>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid AnimalId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VeterinarianId { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.AnimalId)
                .NotEmpty().WithMessage("AnimalId is required.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.VeterinarianId)
                .NotEmpty().WithMessage("VeterinarianId is required.");

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.Now).WithMessage("Start time must be in the future.");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be a valid appointment status.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must be less than 500 characters.")
                .When(x => x.Notes != null);
        }
    }

    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IMapper _mapper;
        private readonly CalendarDbContext _dbContext;
        private readonly IMediator _mediator;

        public CreateAppointmentCommandHandler(CalendarDbContext dbContext, IMediator mediator, IMapper mapper)
        {
            _dbContext = dbContext;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Check if AnimalId exists
            var animalQuery = new GetAnimalByIdQuery(request.AnimalId);
            await _mediator.Send(animalQuery, cancellationToken);

            // Check for duplicate appointments
            var hasOverlap = await _dbContext.Appointments
                .AnyAsync(a => a.AnimalId == request.AnimalId &&
                               a.StartTime <= request.EndTime &&
                               a.EndTime >= request.StartTime, cancellationToken);
            if (hasOverlap)
            {
                throw new ValidationException("An appointment for this animal already exists during the specified time.");
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                AnimalId = request.AnimalId,
                CustomerId = request.CustomerId,
                VeterinarianId = request.VeterinarianId,
                Status = request.Status,
                Notes = request.Notes
            };

            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}