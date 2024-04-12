namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MessageStudent")]
    public partial class MessageStudent
    {
        public long Id { get; set; }

        public int? IdBranch { get; set; }

        public int? IdStudent { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? ReactCount { get; set; }

        [Column(TypeName = "ntext")]
        public string Message { get; set; }

        public bool? IsResponse { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
