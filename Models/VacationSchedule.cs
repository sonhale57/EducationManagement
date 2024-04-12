namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VacationSchedule")]
    public partial class VacationSchedule
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        public DateTime? Fromdate { get; set; }

        public DateTime? Todate { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }
    }
}
