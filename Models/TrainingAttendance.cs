namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingAttendance")]
    public partial class TrainingAttendance
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTraining { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUser { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdWeek { get; set; }

        public DateTime? Updatetime { get; set; }

        [StringLength(500)]
        public string UploadFile { get; set; }

        public int? Score { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }

        public virtual User User { get; set; }
    }
}
