namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ResultCourse")]
    public partial class ResultCourse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? EmailSendCount { get; set; }

        public int? IdUser { get; set; }

        public int? IdStudent { get; set; }

        public int? IdClass { get; set; }

        public int? IdRegistration { get; set; }

        public int? IdCourse { get; set; }

        public DateTime? DateCreate { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(50)]
        public string ScoreBoard { get; set; }

        [StringLength(50)]
        public string Listen { get; set; }

        [StringLength(50)]
        public string View { get; set; }

        [StringLength(50)]
        public string Speed { get; set; }

        [StringLength(50)]
        public string Online { get; set; }

        [StringLength(50)]
        public string TotalScore { get; set; }

        [StringLength(50)]
        public string Focus { get; set; }

        [StringLength(50)]
        public string FocusGet { get; set; }

        [StringLength(50)]
        public string FocusNeed { get; set; }

        [StringLength(50)]
        public string Confident { get; set; }

        [StringLength(50)]
        public string ConfidentGet { get; set; }

        [StringLength(50)]
        public string ConfidentNeed { get; set; }

        [StringLength(50)]
        public string Remember { get; set; }

        [StringLength(50)]
        public string RememberGet { get; set; }

        [StringLength(50)]
        public string RememberNeed { get; set; }

        [StringLength(50)]
        public string Reflex { get; set; }

        [StringLength(50)]
        public string ReflexGet { get; set; }

        [StringLength(50)]
        public string ReflexNeed { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Orentation { get; set; }

        public int? Power { get; set; }

        public virtual Class Class { get; set; }

        public virtual Course Course { get; set; }

        public virtual Registration Registration { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
