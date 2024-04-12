namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserPermission")]
    public partial class UserPermission
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUser { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPermission { get; set; }

        public bool? IsRead { get; set; }

        public bool? IsCreate { get; set; }

        public bool? IsEdit { get; set; }

        public bool? IsDelete { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual User User { get; set; }
    }
}
