namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AchievementEmployee")]
    public partial class AchievementEmployee
    {
        public int Id { get; set; }

        public int? IdEmployee { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [StringLength(50)]
        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual User User { get; set; }
    }
}
