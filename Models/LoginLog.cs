namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoginLog")]
    public partial class LoginLog
    {
        public int Id { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        [StringLength(50)]
        public string Devide { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(50)]
        public string Browser { get; set; }

        public virtual User User { get; set; }
    }
}
