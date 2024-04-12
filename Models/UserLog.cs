namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserLog")]
    public partial class UserLog
    {
        public long Id { get; set; }

        public int? IdUser { get; set; }

        public int? IdLog { get; set; }

        public DateTime? Updatetime { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public virtual Log Log { get; set; }

        public virtual User User { get; set; }
    }
}
