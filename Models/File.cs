namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("File")]
    public partial class File
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public File()
        {
            TrainingDocuments = new HashSet<TrainingDocument>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? IdFolder { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public bool? IsPublic { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        [StringLength(250)]
        public string Link { get; set; }

        public int? IdType { get; set; }

        [StringLength(50)]
        public string FileSize { get; set; }

        public int? AccessFile { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual FileType FileType { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingDocument> TrainingDocuments { get; set; }
    }
}
