namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserAvatar")]
    public partial class UserAvatar
    {
        public int Id { get; set; }

        public int? User_id { get; set; }

        public string Avatar_type { get; set; }
    }
}
