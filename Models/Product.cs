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
        [Display(Name="Tên vật tư")]
        public string Name { get; set; }

        [StringLength(250)]
        [Display(Name="Ghi chú vật tư")]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name="Mã vật tư")]
        public string Code { get; set; }

        [StringLength(500)]
        [Display(Name="Hình ảnh")]
        public string Image { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name="Đơn giá")]
        public decimal? Price { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name="Đơn giá giảm")]
        public decimal? DiscountPrice { get; set; }

        [Display(Name="Trạng thái giảm giá")]
        public bool? StatusDiscount { get; set; }
        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdCategory { get; set; }

        [Display(Name = "Vật tư cốt lõi")]
        public bool? IsCore { get; set; }


        [StringLength(50)]
        [Display(Name="Đơn vị")]
        public string Unit { get; set; }

        [Display(Name="Định mức")]
        public int? Quota { get; set; }

        [Display(Name="Số lượng đóng gói")]
        public int? NumberOfPackage { get; set; }

        [StringLength(50)]
        [Display(Name="Đơn vị đóng gói")]
        public string UnitOfPackage { get; set; }

        public int? Inventory { get; set; }

        [Display(Name="Cho phép cơ sở sửa giá bán")]
        public bool? IsFixed { get; set; }

        [Display(Name="Cho phép bán")]
        public bool? IsSale { get; set; }

        public int? IdSupplier { get; set; }

        [Display(Name="Quy đổi điểm Power")]
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
