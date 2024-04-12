namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Message")]
    public partial class Message
    {
        public long Id { get; set; }

        public int? IdBranch { get; set; }

        public int? FromUser { get; set; }

        public int? ToUser { get; set; }

        public bool? IsPublic { get; set; }

        public bool? Enable { get; set; }

        public DateTime? DateCreate { get; set; }

        [Column("Message", TypeName = "ntext")]
        public string Message1 { get; set; }

        public int? ReactCount { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
