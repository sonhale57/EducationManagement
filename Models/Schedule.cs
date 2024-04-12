namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Schedule")]
    public partial class Schedule
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string IdWeek { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdClass { get; set; }

        public int? IdRoom { get; set; }

        public int? IdEmployee { get; set; }

        public DateTime? FromHour { get; set; }

        public DateTime? ToHour { get; set; }

        public bool? Active { get; set; }

        public int? IdUser { get; set; }

        public virtual Class Class { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Room Room { get; set; }

        public virtual User User { get; set; }
    }
}
