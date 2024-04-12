namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CertificateCategory")]
    public partial class CertificateCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CertificateCategory()
        {
            StudentCertifications = new HashSet<StudentCertification>();
        }

        public int Id { get; set; }

        public int? IdBranch { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentCertification> StudentCertifications { get; set; }
    }
}
