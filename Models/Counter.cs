namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.Counter")]
    public partial class Counter
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        public int Value { get; set; }

        public DateTime? ExpireAt { get; set; }

        [Key]
        [Column(Order = 1)]
        public long Id { get; set; }
    }
}
