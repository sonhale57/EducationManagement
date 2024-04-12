namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LessonCourseOnline")]
    public partial class LessonCourseOnline
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LessonCourseOnline()
        {
            JoinCourseOnlineLogs = new HashSet<JoinCourseOnlineLog>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public int? IdCourse { get; set; }

        [StringLength(500)]
        public string LinkVideo { get; set; }

        [StringLength(500)]
        public string LinkDocument { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public bool? Status { get; set; }

        public virtual CourseOnline CourseOnline { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JoinCourseOnlineLog> JoinCourseOnlineLogs { get; set; }

        public virtual User User { get; set; }
    }
}
