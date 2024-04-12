namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentCheckin")]
    public partial class StudentCheckin
    {
        public int Id { get; set; }

        public int? IdStudent { get; set; }

        public int? IdUser { get; set; }

        public DateTime? DateCreate { get; set; }

        public bool? Enable { get; set; }

        public bool? Status { get; set; }

        public bool? StatusCheckin { get; set; }

        public int? IdClass { get; set; }

        [StringLength(250)]
        public string Lesson { get; set; }

        [StringLength(250)]
        public string Complete { get; set; }

        [StringLength(250)]
        public string Exactly { get; set; }

        public int? Power { get; set; }

        [StringLength(250)]
        public string Other { get; set; }

        [StringLength(250)]
        public string Remember { get; set; }

        [StringLength(250)]
        public string Reflex { get; set; }

        [StringLength(250)]
        public string Confident { get; set; }

        [StringLength(250)]
        public string Focus { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Homework { get; set; }

        [StringLength(250)]
        public string OnClassPaper { get; set; }

        [StringLength(250)]
        public string OnClassNumber { get; set; }

        [StringLength(250)]
        public string OnClassRow { get; set; }

        [StringLength(250)]
        public string HomeComplete { get; set; }

        [StringLength(250)]
        public string HomeExactly { get; set; }

        [StringLength(250)]
        public string PracticeOnline { get; set; }

        [StringLength(500)]
        public string SendSMS { get; set; }

        public virtual Class Class { get; set; }

        public virtual Student Student { get; set; }

        public virtual User User { get; set; }
    }
}
