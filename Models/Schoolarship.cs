namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Schoolarship")]
    public partial class Schoolarship
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Schoolarship()
        {
            SchoolarshipForms = new HashSet<SchoolarshipForm>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Value { get; set; }

        public bool? Enable { get; set; }

        public int? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateExpire { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SchoolarshipForm> SchoolarshipForms { get; set; }
    }
}
