namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Course")]
    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            CourseBranches = new HashSet<CourseBranch>();
            ProductCourses = new HashSet<ProductCourse>();
            RegistrationCourses = new HashSet<RegistrationCourse>();
            ResultCourses = new HashSet<ResultCourse>();
            StudentJoinClasses = new HashSet<StudentJoinClass>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name="Mã khóa học")]
        public string Code { get; set; }

        [StringLength(50)]
        [Display(Name="Tên khóa học")]
        public string Name { get; set; }

        [Display(Name="Số giờ")]
        public int? Hour { get; set; }

        [StringLength(50)]
        [Display(Name="Cấp độ")]
        public string Levels { get; set; }

        [Display(Name="Thứ tự hiển thị")]
        public int? DisplayOrder { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name="Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? Price { get; set; }

        public int? IdProgram { get; set; }

        [StringLength(250)]
        [Display(Name="Mô tả")]
        public string Description { get; set; }

        [StringLength(250)]
        [Display(Name="Công thức giữa kì")]
        public string FormulaMidterm { get; set; }

        [StringLength(50)]
        [Display(Name="Tốc độ giữa kì")]
        public string SpeedMidterm { get; set; }

        [StringLength(250)]
        [Display(Name="Công thức cuối kì")]
        public string FormulaEndterm { get; set; }

        [StringLength(50)]
        [Display(Name="Tốc độ cuối kì")]
        public string SpeedEndterm { get; set; }

        [StringLength(500)]
        [Display(Name="Địn hướng phát triển")]
        public string DevelopRoute { get; set; }

        [Display(Name="Điểm đạt")]
        public int? ScorePass { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [Display(Name="Số buổi")]
        public int? Sessions { get; set; }

        [Display(Name="Số giờ")]
        public int? Hours { get; set; }

        public virtual Program Program { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseBranch> CourseBranches { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductCourse> ProductCourses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistrationCourse> RegistrationCourses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultCourse> ResultCourses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentJoinClass> StudentJoinClasses { get; set; }
    }
}
