namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.Set")]
    public partial class Set
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        public double Score { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(256)]
        public string Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
