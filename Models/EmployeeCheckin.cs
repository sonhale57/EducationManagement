namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeCheckin")]
    public partial class EmployeeCheckin
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public bool? Status { get; set; }

        public int? ApproveBy { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
