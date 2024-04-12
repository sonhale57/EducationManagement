namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public int? IdStudent { get; set; }

        public bool? Enable { get; set; }

        public bool? Status { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
