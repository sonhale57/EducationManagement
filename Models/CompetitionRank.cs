namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompetitionRank")]
    public partial class CompetitionRank
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Area { get; set; }

        public int? Levels { get; set; }

        public int? Rank0 { get; set; }

        public int? Rank1 { get; set; }

        public int? Rank2 { get; set; }

        public int? Rank3 { get; set; }
    }
}
