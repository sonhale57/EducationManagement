namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserJoinCourseOnline")]
    public partial class UserJoinCourseOnline
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUser { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCourse { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? ApproveBy { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public virtual CourseOnline CourseOnline { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
