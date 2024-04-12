namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Configuration")]
    public partial class Configuration
    {
        public int Id { get; set; }

        public int? VAT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? UsageFee { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? AccountFee { get; set; }

        public DateTime? TimePayment { get; set; }

        public int? DiscountGroup1 { get; set; }

        public int? DiscountGroup2 { get; set; }

        public int? DiscountGroup3 { get; set; }

        public int? DiscountGroup4 { get; set; }

        public int? DiscountGroup5 { get; set; }

        public int? DiscountGroup6 { get; set; }

        public int? DiscountGroup7 { get; set; }

        public int? IdUser { get; set; }

        public virtual User User { get; set; }
    }
}
