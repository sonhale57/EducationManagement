namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HangFire.Schema")]
    public partial class Schema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Version { get; set; }
    }
}
