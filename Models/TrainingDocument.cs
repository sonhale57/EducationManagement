namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingDocument")]
    public partial class TrainingDocument
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTraining { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdDocument { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public virtual File File { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }

        public virtual User User { get; set; }
    }
}
