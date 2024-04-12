namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentVoucher")]
    public partial class StudentVoucher
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdStudent { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Voucher { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? IdTransaction { get; set; }

        public bool? Type { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public virtual Student Student { get; set; }

        public virtual Transaction Transaction { get; set; }

        public virtual User User { get; set; }
    }
}
