namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RegistrationTraining")]
    public partial class RegistrationTraining
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTraining { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEmployee { get; set; }

        public int? IdUser { get; set; }

        public DateTime? Updatetime { get; set; }

        public int? Result { get; set; }

        public bool? StatusPayment { get; set; }

        [StringLength(50)]
        public string StatusJoin { get; set; }

        public bool? IsPass { get; set; }

        public int? IdBranch { get; set; }

        public bool? IsRegisteStay { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }
    }
}
