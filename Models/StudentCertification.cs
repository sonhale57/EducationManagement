namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentCertification")]
    public partial class StudentCertification
    {
        public int Id { get; set; }

        public int? IdStudent { get; set; }

        public int? IdCertificate { get; set; }

        public int? IdCourse { get; set; }

        public DateTime? Todate { get; set; }

        public bool? Status { get; set; }

        [StringLength(50)]
        public string TotalScore { get; set; }

        [StringLength(50)]
        public string NumberCertificate { get; set; }

        public virtual CertificateCategory CertificateCategory { get; set; }

        public virtual Student Student { get; set; }
    }
}
