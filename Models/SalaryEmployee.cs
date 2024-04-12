namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalaryEmployee")]
    public partial class SalaryEmployee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? IdEmployee { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public int? IdBranch { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? BasicSalary { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TeachSalary { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Overtime { get; set; }

        public int? NumberOfTeach { get; set; }

        public int? NumberOfWorkday { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Allowance { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? CommissionAllowance { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalSalary { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SalaryDeduction { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalDeduction { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ActualBalance { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SocialSecurity { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? HealthInsurance { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual User User { get; set; }
    }
}
