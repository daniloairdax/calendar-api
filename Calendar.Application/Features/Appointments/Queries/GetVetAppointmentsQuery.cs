using Calendar.Application.Features.Appointments.Models;
using Calendar.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

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
        private readonly CalendarDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetVetAppointmentsQueryHandler(CalendarDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VetAppointmentDto>> Handle(GetVetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Animal)
                .Where(a => a.VeterinarianId == request.VetId &&
                            a.StartTime >= request.StartDate &&
                            a.EndTime <= request.EndDate)
                .ToListAsync(cancellationToken);

            if (appointments == null || appointments.Count == 0)
            {
                throw new NotFoundException("Appointments for veterinarian not found", request.VetId);
            }

            var vetAppointmentsResult = _mapper.Map<IEnumerable<VetAppointmentDto>>(appointments);

            return vetAppointmentsResult;
        }
    }
}
