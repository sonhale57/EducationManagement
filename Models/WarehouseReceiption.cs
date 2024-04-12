namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WarehouseReceiption")]
    public partial class WarehouseReceiption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WarehouseReceiption()
        {
            PProductRecieptionDetails = new HashSet<PProductRecieptionDetail>();
            ProductReceiptionDetails = new HashSet<ProductReceiptionDetail>();
        }

        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Debit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Credit { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public bool? Status { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalAmount { get; set; }

        public bool? Type { get; set; }

        public virtual Branch Branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PProductRecieptionDetail> PProductRecieptionDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductReceiptionDetail> ProductReceiptionDetails { get; set; }

        public virtual User User { get; set; }
    }
}
