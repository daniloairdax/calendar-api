using Calendar.Domain.Enums;
using System;

namespace Calendar.Application.Features.Appointments.Models
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Guid AnimalId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid VeterinarianId { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public string? Notes { get; set; }
    }
}
