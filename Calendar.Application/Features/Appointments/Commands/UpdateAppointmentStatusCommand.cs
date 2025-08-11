using Calendar.Application.Constants;
using Calendar.Application.Exceptions;
using Calendar.Application.Interfaces;
using Calendar.Domain.Enums;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Calendar.Application.Exceptions.ValidationException;

namespace Calendar.Application.Features.Appointments.Commands
{
    public class UpdateAppointmentStatusCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
        public AppointmentStatus Status { get; set; }
    }

    public class UpdateAppointmentStatusCommandValidator : AbstractValidator<UpdateAppointmentStatusCommand>
    {
        public UpdateAppointmentStatusCommandValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("AppointmentId is required.");

            RuleFor(x => x.Status)
                .Must(s => AppConstants.AllowedUpdateAppointmentStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AppConstants.AllowedUpdateAppointmentStatuses)}.");
        }
    }

    public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmailService _emailService;

        public UpdateAppointmentStatusCommandHandler(IAppointmentRepository appointmentRepository, IEmailService emailService)
        {
            _appointmentRepository = appointmentRepository;
            _emailService = emailService;
        }

        public async Task<bool> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

            if (appointment == null)
            {
                throw new NotFoundException("Appointment", request.AppointmentId);
            }

            // Business rule: Cannot cancel within 1 hour of start time
            if (request.Status == AppointmentStatus.Canceled && appointment.StartTime <= DateTime.Now.AddHours(1))
            {
                throw new ValidationException("Cannot cancel within 1 hour of start time.");
            }

            appointment.Status = request.Status;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            // Send email notification on cancel
            if (request.Status == AppointmentStatus.Canceled)
            {
                var recipientEmail = appointment.Animal?.OwnerEmail ?? AppConstants.OWNER_EMAIL;
                var subject = "Appointment Canceled";
                var body = $"Your appointment scheduled for {appointment.StartTime} has been canceled.";
                _emailService.SendEmail(recipientEmail, subject, body);
            }

            return true;
        }
    }
}