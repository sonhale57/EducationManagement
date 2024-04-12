namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [StringLength(250)]
        public string Link { get; set; }

        public int? Type { get; set; }

        public int? Value { get; set; }

        public bool? IsPublic { get; set; }

        public int? IdBranch { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }
    }
}
