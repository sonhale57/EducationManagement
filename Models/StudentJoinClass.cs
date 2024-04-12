namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentJoinClass")]
    public partial class StudentJoinClass
    {
        public int Id { get; set; }

        public int? IdStudent { get; set; }

        public int? IdClass { get; set; }

        public int? IdCourse { get; set; }

        public int? IdRegistration { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public DateTime? Fromdate { get; set; }

        public DateTime? Todate { get; set; }

        public bool? Enable { get; set; }

        public int? Sessions { get; set; }

        public int? MonthFee { get; set; }

        public int? YearFee { get; set; }

        public virtual Class Class { get; set; }

        public virtual Course Course { get; set; }

        public virtual Registration Registration { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
