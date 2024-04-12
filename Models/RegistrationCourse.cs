namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RegistrationCourse")]
    public partial class RegistrationCourse
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdRegistration { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCourse { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalAmount { get; set; }

        public bool? Status { get; set; }

        public int? Amount { get; set; }

        public bool? Enable { get; set; }

        public bool? StatusExchangeCourse { get; set; }

        public DateTime? DateExchangeCourse { get; set; }

        public bool? StatusExtend { get; set; }

        public DateTime? DateExtend { get; set; }

        public bool? StatusReserve { get; set; }

        public DateTime? DateReserve { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public bool? StatusJoinClass { get; set; }

        public DateTime? DateJoinClass { get; set; }

        public virtual Course Course { get; set; }

        public virtual Registration Registration { get; set; }
    }
}
