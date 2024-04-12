namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BranchCommission")]
    public partial class BranchCommission
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Percent { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Money { get; set; }

        public bool? Enable { get; set; }

        public int? IdBranch { get; set; }

        public virtual Branch Branch { get; set; }
    }
}
