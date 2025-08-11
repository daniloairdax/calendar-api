using Calendar.Domain.Enums;

namespace Calendar.Application.Constants
{
    public static class AppConstants
    {
        public const string OWNER_EMAIL = "owner@example.com";
        public static readonly AppointmentStatus[] AllowedUpdateAppointmentStatuses = { AppointmentStatus.Scheduled, AppointmentStatus.Completed, AppointmentStatus.Canceled };
    }
}
