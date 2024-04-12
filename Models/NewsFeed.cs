namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NewsFeed")]
    public partial class NewsFeed
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NewsFeed()
        {
            ReactionNewsFeeds = new HashSet<ReactionNewsFeed>();
        }

        public int Id { get; set; }

        [StringLength(500)]
        public string Thumbnail { get; set; }

        [Column(TypeName = "ntext")]
        public string Context { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        public bool? Enable { get; set; }

        public bool? IsPublish { get; set; }

        public int? ReactionCount { get; set; }

        public int? ToGroup { get; set; }

        public virtual GroupChat GroupChat { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionNewsFeed> ReactionNewsFeeds { get; set; }
    }
}
