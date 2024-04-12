namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Competition")]
    public partial class Competition
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdStudent { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Year { get; set; }

        public int? IdBranch { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public bool? Status { get; set; }

        public bool? Enable { get; set; }

        public int? Levels { get; set; }

        [StringLength(50)]
        public string Board { get; set; }

        [StringLength(50)]
        public string Seat { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public double? ScoreSet1 { get; set; }

        public double? ScoreSet2 { get; set; }

        public double? ScoreSet3 { get; set; }

        public double? ScoreSet4 { get; set; }

        public double? ScoreSet5 { get; set; }

        public double? ScoreSet6 { get; set; }

        public bool? StatusCheckin { get; set; }

        public DateTime? TimeCheckin { get; set; }

        public int? UserCheckin { get; set; }

        [StringLength(50)]
        public string Area { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
