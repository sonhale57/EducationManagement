namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.Hash")]
    public partial class Hash
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Field { get; set; }

        public string Value { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ExpireAt { get; set; }
    }
}
