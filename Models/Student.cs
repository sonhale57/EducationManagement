namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Student")]
    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            Competitions = new HashSet<Competition>();
            Feedbacks = new HashSet<Feedback>();
            MessageStudents = new HashSet<MessageStudent>();
            Registrations = new HashSet<Registration>();
            ResultCourses = new HashSet<ResultCourse>();
            StudentAdvises = new HashSet<StudentAdvise>();
            StudentCertifications = new HashSet<StudentCertification>();
            StudentCheckins = new HashSet<StudentCheckin>();
            StudentInputTests = new HashSet<StudentInputTest>();
            StudentJoinClasses = new HashSet<StudentJoinClass>();
            StudentVouchers = new HashSet<StudentVoucher>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Họ tên")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh đại diện")]
        public string Image { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã học viên")]
        public string Code { get; set; }

        [Display(Name = "Sinh nhật")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string Sex { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [StringLength(250)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        public bool? Enable { get; set; }

        [StringLength(50)]
        [Display(Name = "Trường")]
        public string School { get; set; }

        [StringLength(50)]
        [Display(Name = "Lớp")]
        public string Class { get; set; }

        [StringLength(250)]
        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Họ tên phụ huynh")]
        public string ParentName { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [Display(Name = "Sinh nhật phụ huynh")]
        public DateTime? ParentDateOfBirth { get; set; }

        [StringLength(50)]
        [Display(Name = "Tỉnh/Thành phố")]
        public string City { get; set; }

        [StringLength(50)]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [StringLength(50)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [StringLength(50)]
        [Display(Name = "Mối quan hệ với phụ huynh")]
        public string Relationship { get; set; }

        [StringLength(50)]
        [Display(Name = "Công việc")]
        public string Job { get; set; }

        [StringLength(50)]
        public string Facebook { get; set; }

        [StringLength(50)]
        [Display(Name = "Mong muốn")]
        public string Hopeful { get; set; }

        [StringLength(50)]
        [Display(Name = "Biết Superbrain qua")]
        public string Known { get; set; }

        [StringLength(10)]
        public string IdMKT { get; set; }
            
        public int? IdBranch { get; set; }

        public int? PowerScore { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Balance { get; set; }

        public int? Presenter { get; set; }

        public bool? Status { get; set; }

        public int? Power { get; set; }

        public int? StatusStudy { get; set; }
        public DateTime DateCreate { get; set; }
        public int? IdUser { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Competition> Competitions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageStudent> MessageStudents { get; set; }

        public virtual MKTCampaign MKTCampaign { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Registration> Registrations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultCourse> ResultCourses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentAdvise> StudentAdvises { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCertification> StudentCertifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCheckin> StudentCheckins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentInputTest> StudentInputTests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentJoinClass> StudentJoinClasses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentVoucher> StudentVouchers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
