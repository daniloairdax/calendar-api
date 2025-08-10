using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Features.Appointments.Queries;
using Calendar.Application.Interfaces;
using Calendar.Domain.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.Tests.Features.Appointments
{
    [ExcludeFromCodeCoverage]
    public class GetVetAppointmentsQueryTests
    {
        private GetVetAppointmentsQuery _query;
        private readonly GetVetAppointmentsQueryValidator _validator;
        private readonly IMapper _mapper;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetVetAppointmentsQueryTests()
        {
            _validator = new GetVetAppointmentsQueryValidator();
            _mapper = Substitute.For<IMapper>();
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
        }

        [Fact]
        public async Task Handle_ReturnsAppointments_WhenFound()
        {
            // Arrange
            var vetId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);
            _query = new GetVetAppointmentsQuery(vetId, startDate, endDate);
            var appointments = new List<Appointment>
            {
                new Appointment { VeterinarianId = vetId, StartTime = startDate.AddHours(1), EndTime = endDate.AddHours(-1), Animal = new Animal { Name = "Dog", OwnerName = "Owner" }, Status = Domain.Enums.AppointmentStatus.Scheduled }
            };
            _appointmentRepository.GetByVetAndDateRangeAsync(vetId, startDate, endDate, Arg.Any<CancellationToken>()).Returns(appointments);
            _mapper.Map<IEnumerable<VetAppointmentDto>>(appointments).Returns(new List<VetAppointmentDto> { new VetAppointmentDto { AnimalName = "Dog", OwnerName = "Owner" } });
            var handler = new GetVetAppointmentsQueryHandler(_appointmentRepository, _mapper);

            // Act
            var result = await handler.Handle(_query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Dog", ((List<VetAppointmentDto>)result)[0].AnimalName);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenNoAppointmentsFound()
        {
            // Arrange
            var vetId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);
            _query = new GetVetAppointmentsQuery(vetId, startDate, endDate);
            var handler = new GetVetAppointmentsQueryHandler(_appointmentRepository, _mapper);
            _appointmentRepository.GetByVetAndDateRangeAsync(vetId, startDate, endDate, Arg.Any<CancellationToken>()).Returns(new List<Appointment>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(_query, CancellationToken.None));
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "2025-01-01", "2025-01-02", "VetId is required.")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d481", "2025-01-02", "2025-01-01", "StartDate must be before EndDate.")]
        public void GetVetAppointmentsQueryValidator_Throws_ForInvalidInputs(string vetId, string start, string end, string expectedError)
        {
            // Arrange
            _query = new GetVetAppointmentsQuery(Guid.Parse(vetId), DateTime.Parse(start), DateTime.Parse(end));

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void GetVetAppointmentsQueryValidator_Passes_WhenValid()
        {
            // Arrange
            var vetId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);
            _query = new GetVetAppointmentsQuery(vetId, startDate, endDate);

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
