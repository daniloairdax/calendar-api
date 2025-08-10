using AutoMapper;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Appointments.Queries
{
    public class GetAppointmentByIdQuery : IRequest<AppointmentDto?>
    {
        public Guid Id { get; }

        public GetAppointmentByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
    {
        public GetAppointmentByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Appointment Id is required.");
        }
    }

    public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
    {
        private readonly CalendarDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAppointmentByIdQueryHandler(CalendarDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .Include(a => a.Animal)
                .Where(a => a.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (appointment == null)
            {
                throw new NotFoundException("Appointment", request.Id);
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}