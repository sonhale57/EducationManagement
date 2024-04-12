namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReactionNewsFeed")]
    public partial class ReactionNewsFeed
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdNewsFeed { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUser { get; set; }

        public int? Reaction { get; set; }

        public DateTime? DateCreate { get; set; }

        public virtual NewsFeed NewsFeed { get; set; }

        public virtual User User { get; set; }
    }
}
