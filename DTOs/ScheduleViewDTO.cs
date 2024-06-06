namespace SuperbrainManagement.DTOs
{
    public class ScheduleViewDTO
    {
        public bool Active { get; set; } 

        public string FromHour { get; set; } 

        public string ToHour { get; set; } 

        public int EmployeeId { get; set; } 

        public int RoomId { get; set; }

        public string IdWeek { get; set; }

        public string IdClass { get; set; }

        public string IdUser { get; set; }
    }
}