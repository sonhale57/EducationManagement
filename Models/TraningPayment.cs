namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TraningPayment")]
    public partial class TraningPayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? IdBranch { get; set; }

        public int? IdTraining { get; set; }

        public int? IdUser { get; set; }

        public DateTime? Updatetime { get; set; }

        [StringLength(50)]
        public string CodePayment { get; set; }

        [StringLength(50)]
        public string KeyPayment { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public bool? Status { get; set; }

        public int? Number { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Amount { get; set; }

        [StringLength(10)]
        public string Descript { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }

        public virtual User User { get; set; }
    }
}
