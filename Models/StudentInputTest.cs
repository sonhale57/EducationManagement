namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentInputTest")]
    public partial class StudentInputTest
    {
        public int Id { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        [StringLength(50)]
        public string Result { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Focus { get; set; }

        [StringLength(50)]
        public string Reflex { get; set; }

        public int? IdStudent { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
