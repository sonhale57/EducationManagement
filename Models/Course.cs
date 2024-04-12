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
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Hour { get; set; }

        [StringLength(50)]
        public string Levels { get; set; }

        public int? DisplayOrder { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        public int? IdProgram { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(250)]
        public string FormulaMidterm { get; set; }

        [StringLength(50)]
        public string SpeedMidterm { get; set; }

        [StringLength(250)]
        public string FormulaEndterm { get; set; }

        [StringLength(50)]
        public string SpeedEndterm { get; set; }

        [StringLength(500)]
        public string DevelopRoute { get; set; }

        public int? ScorePass { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? Sessions { get; set; }

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
