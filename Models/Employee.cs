namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employee")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            AchievementEmployees = new HashSet<AchievementEmployee>();
            RegistrationTrainings = new HashSet<RegistrationTraining>();
            SalaryEmployees = new HashSet<SalaryEmployee>();
            Schedules = new HashSet<Schedule>();
            TrainingResults = new HashSet<TrainingResult>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Họ tên")]
        public string Name { get; set; }

        [Display(Name = "Vị trí")]
        public int? IdPosition { get; set; }

        [StringLength(250)]
        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh đại diện")]
        public string Image { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [StringLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Số giấy chứng nhận")]
        public int? CertificateNumber { get; set; }

        [Display(Name = "Thời gian Giấy chứng nhận")]
        public DateTime? DateCertificate { get; set; }

        [Display(Name = "Thời gian bắt đầu công tác")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? StartWork { get; set; }

        [Display(Name = "Cơ sở")]
        public int? IdBranch { get; set; }

        [Display(Name = "Chính thức")]
        public bool? IsOfficial { get; set; }

        public bool? Enable { get; set; }

        [Display(Name = "Sinh nhật")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [StringLength(50)]
        [Display(Name = "Trình độ")]
        public string Gratuate { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string Sex { get; set; }

        public DateTime? DateCreate { get; set; }
        public DateTime? DateStart { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Lương cơ bản")]
        public decimal? BasicSalary { get; set; }

        [StringLength(50)]
        [Display(Name = "Ngân hàng")]
        public string BankName { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên tài khoản")]
        public string BankAccountName { get; set; }

        [StringLength(50)]
        [Display(Name = "Số tài khoản")]
        public string BankNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Chi nhánh")]
        public string BankBranch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AchievementEmployee> AchievementEmployees { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Position Position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistrationTraining> RegistrationTrainings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalaryEmployee> SalaryEmployees { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schedule> Schedules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingResult> TrainingResults { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
