namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SchoolarshipForm")]
    public partial class SchoolarshipForm
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdSchoolarship { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdStudent { get; set; }

        public DateTime? DateCreate { get; set; }

        public int? IdUser { get; set; }

        public bool? Status { get; set; }

        public int? ApproveBy { get; set; }

        public DateTime? ApproveDate { get; set; }

        public virtual Schoolarship Schoolarship { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
