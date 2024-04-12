namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductCourse")]
    public partial class ProductCourse
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdCourse { get; set; }

        public int? IdProduct { get; set; }

        public int? Amount { get; set; }

        public bool? Enable { get; set; }

        public bool? Status { get; set; }

        public virtual Course Course { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
