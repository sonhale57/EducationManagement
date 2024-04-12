namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CourseBranch")]
    public partial class CourseBranch
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdBranch { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCourse { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PriceCourse { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PriceAccount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PriceTest { get; set; }

        public int? Hour { get; set; }

        public int? Sessons { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DiscountPrice { get; set; }

        public bool? StatusDiscount { get; set; }

        public DateTime? FromdateDiscount { get; set; }

        public DateTime? TodateDiscount { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Course Course { get; set; }
    }
}
