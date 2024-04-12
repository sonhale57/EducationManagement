namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MessageGroup")]
    public partial class MessageGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? IdGroup { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        [Column(TypeName = "ntext")]
        public string Message { get; set; }

        public virtual GroupChat GroupChat { get; set; }

        public virtual User User { get; set; }
    }
}
