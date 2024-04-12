namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BranchGroup")]
    public partial class BranchGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BranchGroup()
        {
            Branches = new HashSet<Branch>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? Enable { get; set; }

        public DateTime? DateCreate { get; set; }

        [StringLength(50)]
        public string NameInvestor { get; set; }

        [StringLength(20)]
        public string PhoneInvestor { get; set; }

        [StringLength(50)]
        public string EmailInvestor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Branch> Branches { get; set; }
    }
}
