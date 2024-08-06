namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingCourse")]
    public partial class TrainingCourse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingCourse()
        {
            RegistrationTrainings = new HashSet<RegistrationTraining>();
            TrainingAttendances = new HashSet<TrainingAttendance>();
            TrainingDocuments = new HashSet<TrainingDocument>();
            TrainingResults = new HashSet<TrainingResult>();
            TraningPayments = new HashSet<TraningPayment>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã")]
        public string Code { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime? Fromdate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime? Todate { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        [Display(Name = "Hạn đăng ký")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ResgistrationDeadline { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Phí tham gia")]
        public decimal? Price { get; set; }

        [Display(Name = "Số lượng")]
        public int? Number { get; set; }

        public int? IdType { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistrationTraining> RegistrationTrainings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingAttendance> TrainingAttendances { get; set; }

        public virtual TrainingType TrainingType { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingDocument> TrainingDocuments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingResult> TrainingResults { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TraningPayment> TraningPayments { get; set; }
    }
}
