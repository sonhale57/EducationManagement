namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportForm")]
    public partial class ReportForm
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? Observer { get; set; }

        public int? ApproveBy { get; set; }

        public DateTime? ApproveDate { get; set; }

        [StringLength(500)]
        public string Attachment { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }
    }
}
