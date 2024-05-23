namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Program")]
    public partial class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Program()
        {
            Courses = new HashSet<Course>();
            CourseOnlines = new HashSet<CourseOnline>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã chương trình")]
        public string Code { get; set; }

        [StringLength(50)]
        [Display(Name="Tên chương trình")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name="Mô tả")]
        public string Description { get; set; }

        [Display(Name="Thứ tự hiển thị")]
        public int? DisplayOrder { get; set; }

        [Display(Name="Bài test đầu vào")]
        public bool? IsTest { get; set; }

        public bool? Enable { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOnline> CourseOnlines { get; set; }

        public virtual User User { get; set; }
    }
}
