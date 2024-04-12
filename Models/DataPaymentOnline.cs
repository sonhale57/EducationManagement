namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DataPaymentOnline")]
    public partial class DataPaymentOnline
    {
        public int Id { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public int? IdStudent { get; set; }

        public int? IdCourse { get; set; }

        public DateTime? Fromdate { get; set; }

        public DateTime? Todate { get; set; }

        public DateTime? Updatetime { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public virtual User User { get; set; }
    }
}
