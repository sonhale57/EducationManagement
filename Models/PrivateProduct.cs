namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PrivateProduct")]
    public partial class PrivateProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrivateProduct()
        {
            PProductRecieptionDetails = new HashSet<PProductRecieptionDetail>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? IdCategory { get; set; }

        public int? IdSupplier { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        public int? Quota { get; set; }

        public int? Inventory { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        public virtual Branch Branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PProductRecieptionDetail> PProductRecieptionDetails { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        public virtual User User { get; set; }
    }
}
