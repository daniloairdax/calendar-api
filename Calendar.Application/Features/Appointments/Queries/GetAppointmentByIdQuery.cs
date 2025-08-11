using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Interfaces;
using FluentValidation;
using MediatR;
using System;
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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (appointment == null)
            {
                throw new NotFoundException("Appointment", request.Id);
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}