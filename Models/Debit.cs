namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Debit")]
    public partial class Debit
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalAmount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Payment { get; set; }

        public bool? Type { get; set; }

        public bool? Status { get; set; }

        public DateTime? Todate { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? IdRegistration { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Registration Registration { get; set; }

        public virtual User User { get; set; }
    }
}
