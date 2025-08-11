using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Features.Appointments.Queries;
using Calendar.Application.Interfaces;
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
    public class GetAppointmentByIdQueryTests
    {
        private GetAppointmentByIdQuery _query;
        private readonly GetAppointmentByIdQueryValidator _validator;
        private readonly IMapper _mapper;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentByIdQueryTests()
        {
            _validator = new GetAppointmentByIdQueryValidator();
            _mapper = Substitute.For<IMapper>();
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
        }

        [Fact]
        public async Task Handle_ReturnsAppointmentDto_WhenAppointmentExists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _query = new GetAppointmentByIdQuery(appointmentId);
            var appointment = new Appointment { Id = appointmentId, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), Animal = new Animal { Name = "Dog" } };
            _appointmentRepository.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>()).Returns(appointment);
            var appointmentDto = new AppointmentDto { Id = appointmentId, StartTime = appointment.StartTime, EndTime = appointment.EndTime, AnimalId = appointment.AnimalId };
            _mapper.Map<AppointmentDto>(appointment).Returns(appointmentDto);
            var handler = new GetAppointmentByIdQueryHandler(_appointmentRepository, _mapper);

            // Act
            var result = await handler.Handle(_query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.Id);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _query = new GetAppointmentByIdQuery(appointmentId);
            _appointmentRepository.GetByIdAsync(appointmentId, Arg.Any<CancellationToken>()).Returns((Appointment)null);
            var handler = new GetAppointmentByIdQueryHandler(_appointmentRepository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(_query, CancellationToken.None));
        }

        [Fact]
        public void GetAppointmentByIdQueryValidator_Throws_WhenIdIsEmpty()
        {
            // Arrange
            _query = new GetAppointmentByIdQuery(Guid.Empty);

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Id");
        }

        [Fact]
        public void GetAppointmentByIdQueryValidator_Passes_WhenIdIsValid()
        {
            // Arrange
            var validId = Guid.NewGuid();
            _query = new GetAppointmentByIdQuery(validId);

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
