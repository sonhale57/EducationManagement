namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentAdvise")]
    public partial class StudentAdvise
    {
        public int Id { get; set; }

        public int? IdStudent { get; set; }

        public int? DateCreate { get; set; }

        public DateTime? AppointmentDate { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Result { get; set; }

        public int? IdUser { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
