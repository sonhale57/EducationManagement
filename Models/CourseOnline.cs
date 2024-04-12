namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CourseOnline")]
    public partial class CourseOnline
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseOnline()
        {
            LessonCourseOnlines = new HashSet<LessonCourseOnline>();
            UserJoinCourseOnlines = new HashSet<UserJoinCourseOnline>();
            VideoCourseOnlines = new HashSet<VideoCourseOnline>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdProgram { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DiscountPrice { get; set; }

        public bool? StatusDiscount { get; set; }

        public bool? IsPublic { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public virtual Program Program { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LessonCourseOnline> LessonCourseOnlines { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserJoinCourseOnline> UserJoinCourseOnlines { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VideoCourseOnline> VideoCourseOnlines { get; set; }
    }
}
