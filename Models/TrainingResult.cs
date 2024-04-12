namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingResult")]
    public partial class TrainingResult
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEmpoyee { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTraining { get; set; }

        public bool Result { get; set; }

        public int? TotalScore { get; set; }

        public int? NumberCertification { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        [StringLength(50)]
        public string ProfessionalScore { get; set; }

        [StringLength(50)]
        public string SuperbrainScore { get; set; }

        [StringLength(50)]
        public string BrandScore { get; set; }

        [StringLength(50)]
        public string TeachScore { get; set; }

        [StringLength(50)]
        public string SaleScore { get; set; }

        [StringLength(50)]
        public string MindsetScore { get; set; }

        [StringLength(50)]
        public string SorobanScore { get; set; }

        [StringLength(50)]
        public string OnlineScore { get; set; }

        [StringLength(50)]
        public string OfflineScore { get; set; }

        [StringLength(50)]
        public string CompleteScore { get; set; }

        [StringLength(50)]
        public string ParticipationScore { get; set; }

        [StringLength(50)]
        public string DemeanorScore { get; set; }

        [StringLength(50)]
        public string ProactiveScore { get; set; }

        [StringLength(50)]
        public string Focus { get; set; }

        [StringLength(50)]
        public string Reflex { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual TrainingCourse TrainingCourse { get; set; }

        public virtual User User { get; set; }
    }
}
