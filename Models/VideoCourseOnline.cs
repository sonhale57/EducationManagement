namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VideoCourseOnline")]
    public partial class VideoCourseOnline
    {
        public int Id { get; set; }

        public int? IdCourse { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [StringLength(500)]
        public string Link { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public virtual CourseOnline CourseOnline { get; set; }

        public virtual User User { get; set; }
    }
}
