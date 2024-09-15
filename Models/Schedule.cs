namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Schedule")]
    public partial class Schedule
    {
        [Key]
        [Column(Order = 0)]
        public int IdWeek { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdClass { get; set; }

        public int? IdRoom { get; set; }

        public int? IdEmployee { get; set; }

        public DateTime? FromHour { get; set; }

        public DateTime? ToHour { get; set; }

        public bool? Active { get; set; }

        public int? IdUser { get; set; }

        public virtual Class Class { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Room Room { get; set; }

        public virtual User User { get; set; }
    }
    public class ScheduleUpdateModel
    {
        public int ScheduleId { get; set; } // Id của schedule
        public int EmployeeId { get; set; } // Id giáo viên được chọn
        public int RoomId { get; set; } // Id phòng học được chọn
        public bool IsActive { get; set; } // Trạng thái checkbox
        public int ClassId { get; set; }
        public DateTime FromHour { get; set; }
        public DateTime ToHour { get; set; }
    }
}
