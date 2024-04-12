namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.JobParameter")]
    public partial class JobParameter
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JobId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string Name { get; set; }

        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}
