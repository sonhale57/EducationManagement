namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductCategory")]
    public partial class ProductCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductCategory()
        {
            PrivateProducts = new HashSet<PrivateProduct>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã danh mục")]
        public string Code { get; set; }


        [StringLength(50)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }


        [StringLength(250)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        public bool? Enable { get; set; }
        [Display(Name = "Trạng thái")]
        public bool? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int? DisplayOrder { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivateProduct> PrivateProducts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
