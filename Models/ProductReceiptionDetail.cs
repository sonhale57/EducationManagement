namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductReceiptionDetail")]
    public partial class ProductReceiptionDetail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdReceiption { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProduct { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalAmount { get; set; }

        public bool? Status { get; set; }

        public int? IdSupplier { get; set; }

        public bool? Type { get; set; }
        public int Amount { get; set; }

        public virtual Product Product { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual WarehouseReceiption WarehouseReceiption { get; set; }
    }
}
