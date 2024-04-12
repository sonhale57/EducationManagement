namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Transaction")]
    public partial class Transaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transaction()
        {
            StudentVouchers = new HashSet<StudentVoucher>();
        }

        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdStudent { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? Type { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalAmount { get; set; }

        public bool? Status { get; set; }

        public int? IdBranch { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        public int? IdRegistration { get; set; }

        public int? IdOrder { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Order Order { get; set; }

        public virtual Registration Registration { get; set; }

        public virtual Student Student { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentVoucher> StudentVouchers { get; set; }

        public virtual User User { get; set; }
    }
}
