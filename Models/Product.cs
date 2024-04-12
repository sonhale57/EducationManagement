namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductCourses = new HashSet<ProductCourse>();
            ProductPromotions = new HashSet<ProductPromotion>();
            ProductReceiptionDetails = new HashSet<ProductReceiptionDetail>();
            RegistrationProducts = new HashSet<RegistrationProduct>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DiscountPrice { get; set; }

        public bool? StatusDiscount { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdCategory { get; set; }

        public bool? IsCore { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        public int? Quota { get; set; }

        public int? NumberOfPackage { get; set; }

        [StringLength(50)]
        public string UnitOfPackage { get; set; }

        public int? Inventory { get; set; }

        public bool? IsFixed { get; set; }

        public bool? IsSale { get; set; }

        public int? IdSupplier { get; set; }

        public int? PowerScore { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductCourse> ProductCourses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductReceiptionDetail> ProductReceiptionDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistrationProduct> RegistrationProducts { get; set; }
    }
}
