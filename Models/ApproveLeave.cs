namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ApproveLeave")]
    public partial class ApproveLeave
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public DateTime? Fromdate { get; set; }

        public DateTime? Todate { get; set; }

        public int? ApproveBy { get; set; }

        public DateTime? ApproveDate { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? Status { get; set; }

        public bool? Enable { get; set; }

        public int? IdBranch { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
