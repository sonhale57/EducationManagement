namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FAQ")]
    public partial class FAQ
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [StringLength(250)]
        public string Question { get; set; }

        [Column(TypeName = "ntext")]
        public string Answer { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public virtual User User { get; set; }
    }
}
