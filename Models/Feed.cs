namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feed")]
    public partial class Feed
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public DateTime? Todate { get; set; }

        public bool? IsPublic { get; set; }

        public int? Type { get; set; }

        public virtual User User { get; set; }
    }
}
