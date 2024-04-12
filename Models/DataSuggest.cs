namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataSuggest")]
    public partial class DataSuggest
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? Type { get; set; }

        public int? Value { get; set; }

        public int? DisplayOrder { get; set; }

        public int? IdBranch { get; set; }

        public bool? Enable { get; set; }

        public bool? Active { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual User User { get; set; }
    }
}
