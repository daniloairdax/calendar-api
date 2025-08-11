using Calendar.Application.Exceptions;
using Calendar.Application.Features.Appointments.Commands;
using Calendar.Application.Interfaces;
using Calendar.Domain.Enums;
using Calendar.Domain.Models;
using NSubstitute;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.Tests.Features.Appointments
{
    [ExcludeFromCodeCoverage]
    public class UpdateAppointmentStatusCommandTests
    {
        private UpdateAppointmentStatusCommand _command;
        private readonly UpdateAppointmentStatusCommandValidator _validator;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmailService _emailService;

        public UpdateAppointmentStatusCommandTests()
        {
            _validator = new UpdateAppointmentStatusCommandValidator();
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
            _emailService = Substitute.For<IEmailService>();
        }

        [Fact]
        public async Task Handle_UpdatesStatus_SendsEmailOnCancel()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _command = new UpdateAppointmentStatusCommand
            {
                AppointmentId = appointmentId,
                Status = AppointmentStatus.Canceled
            };
            var appointment = new Appointment
            {
                Id = appointmentId,
                StartTime = DateTime.Now.AddHours(2),
                Status = AppointmentStatus.Scheduled,
                Animal = new Animal { OwnerEmail = "owner@example.com" }
            };
            _appointmentRepository.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>()).Returns(appointment);
            _appointmentRepository.UpdateAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepository, _emailService);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _emailService.Received(1).SendEmail("owner@example.com", "Appointment Canceled", Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenCancelWithinOneHour()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _command = new UpdateAppointmentStatusCommand
            {
                AppointmentId = appointmentId,
                Status = AppointmentStatus.Canceled
            };
            var appointment = new Appointment
            {
                Id = appointmentId,
                StartTime = DateTime.UtcNow.AddMinutes(30),
                Status = AppointmentStatus.Scheduled,
                Animal = new Animal { OwnerEmail = "owner@example.com" }
            };
            _appointmentRepository.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>()).Returns(appointment);
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepository, _emailService);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(_command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _command = new UpdateAppointmentStatusCommand
            {
                AppointmentId = appointmentId,
                Status = AppointmentStatus.Scheduled
            };
            _appointmentRepository.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>()).Returns((Appointment)null);
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepository, _emailService);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(_command, CancellationToken.None));
        }

        [Fact]
        public void UpdateAppointmentStatusCommandValidator_Throws_WhenIdIsEmpty()
        {
            // Arrange
            _command = new UpdateAppointmentStatusCommand { AppointmentId = Guid.Empty, Status = AppointmentStatus.Scheduled };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "AppointmentId");
        }

        [Fact]
        public void UpdateAppointmentStatusCommandValidator_Passes_WhenValid()
        {
            // Arrange
            _command = new UpdateAppointmentStatusCommand { AppointmentId = Guid.NewGuid(), Status = AppointmentStatus.Scheduled };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void UpdateAppointmentStatusCommandValidator_Throws_WhenStatusIsNotAllowed()
        {
            // Arrange
            var command = new UpdateAppointmentStatusCommand
            {
                AppointmentId = Guid.NewGuid(),
                Status = AppointmentStatus.InProgress // Not allowed
            };
            var validator = new UpdateAppointmentStatusCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Status");
        }
    }
}