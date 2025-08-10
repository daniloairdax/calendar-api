using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar.Application.Features.Appointments.Queries
{
    public class GetVetAppointmentsQuery : IRequest<IEnumerable<VetAppointmentDto>>
    {
        public Guid VetId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public GetVetAppointmentsQuery(Guid vetId, DateTime startDate, DateTime endDate)
        {
            VetId = vetId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public class GetVetAppointmentsQueryValidator : AbstractValidator<GetVetAppointmentsQuery>
    {
        public GetVetAppointmentsQueryValidator()
        {
            RuleFor(x => x.VetId).NotEmpty().WithMessage("VetId is required.");
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate.");
        }
    }

    public class GetVetAppointmentsQueryHandler : IRequestHandler<GetVetAppointmentsQuery, IEnumerable<VetAppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetVetAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VetAppointmentDto>> Handle(GetVetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetByVetAndDateRangeAsync(
                request.VetId, request.StartDate, request.EndDate, cancellationToken);

            if (appointments == null || !appointments.Any())
            {
                throw new NotFoundException("Appointments for veterinarian not found", request.VetId);
            }

            return _mapper.Map<IEnumerable<VetAppointmentDto>>(appointments);
        }
    }
}