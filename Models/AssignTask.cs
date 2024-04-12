namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AssignTask")]
    public partial class AssignTask
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTask { get; set; }

        public int? IdUser { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AssignTo { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? Deadline { get; set; }

        public virtual Task Task { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
