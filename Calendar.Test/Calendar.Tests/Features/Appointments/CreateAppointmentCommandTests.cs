using AutoMapper;
using Calendar.Application.Features.Appointments.Commands;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Interfaces;
using Calendar.Domain.Enums;
using Calendar.Domain.Models;
using MediatR;
using NSubstitute;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.Tests.Features.Appointments
{
    [ExcludeFromCodeCoverage]
    public class CreateAppointmentCommandTests
    {
        private CreateAppointmentCommand _command;
        private readonly CreateAppointmentCommandValidator _validator;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAppointmentRepository _appointmentRepository;

        public CreateAppointmentCommandTests()
        {
            _validator = new CreateAppointmentCommandValidator();
            _mapper = Substitute.For<IMapper>();
            _mediator = Substitute.For<IMediator>();
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
        }

        [Fact]
        public async Task Handle_CreatesAppointment_ReturnsAppointmentDto()
        {
            // Arrange
            _command = new CreateAppointmentCommand
            {
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                AnimalId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                VeterinarianId = Guid.NewGuid(),
                Status = AppointmentStatus.Scheduled,
                Notes = "Checkup"
            };
            _mediator.Send(Arg.Any<CreateAppointmentCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new AppointmentDto { StartTime = _command.StartTime }));
            _appointmentRepository.AddAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            _mapper.Map<AppointmentDto>(Arg.Any<Appointment>()).Returns(new AppointmentDto { StartTime = _command.StartTime });

            var handler = new CreateAppointmentCommandHandler(_appointmentRepository, _mediator, _mapper);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_command.StartTime, result.StartTime);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d481", "2025-01-01", "2025-01-02", "Scheduled", "AnimalId is required.")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "00000000-0000-0000-0000-000000000000", "f47ac10b-58cc-4372-a567-0e02b2c3d481", "2025-01-01", "2025-01-02", "Scheduled", "CustomerId is required.")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "00000000-0000-0000-0000-000000000000", "2025-01-01", "2025-01-02", "Scheduled", "VeterinarianId is required.")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d481", "2020-01-01", "2025-01-02", "Scheduled", "Start time must be in the future.")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "f47ac10b-58cc-4372-a567-0e02b2c3d481", "2025-01-02", "2025-01-01", "Scheduled", "End time must be after start time.")]
        public void CreateAppointmentCommandValidator_Throws_ForInvalidInputs(string animalId, string customerId, string vetId, string start, string end, string status, string expectedError)
        {
            // Arrange
            _command = new CreateAppointmentCommand
            {
                AnimalId = Guid.Parse(animalId),
                CustomerId = Guid.Parse(customerId),
                VeterinarianId = Guid.Parse(vetId),
                StartTime = DateTime.Parse(start),
                EndTime = DateTime.Parse(end),
                Status = Enum.Parse<AppointmentStatus>(status),
                Notes = "Test"
            };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void CreateAppointmentCommandValidator_Passes_WhenValid()
        {
            // Arrange
            _command = new CreateAppointmentCommand
            {
                AnimalId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                VeterinarianId = Guid.NewGuid(),
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                Status = AppointmentStatus.Scheduled,
                Notes = "Test"
            };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}