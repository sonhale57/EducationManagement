namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.JobQueue")]
    public partial class JobQueue
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        public long JobId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Queue { get; set; }

        public DateTime? FetchedAt { get; set; }
    }
}
