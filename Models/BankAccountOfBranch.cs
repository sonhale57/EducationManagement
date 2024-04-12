namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BankAccountOfBranch")]
    public partial class BankAccountOfBranch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string BankName { get; set; }

        [StringLength(50)]
        public string BankAccountName { get; set; }

        [StringLength(50)]
        public string BankNumber { get; set; }

        [StringLength(50)]
        public string BankBranch { get; set; }

        [StringLength(500)]
        public string QRCode { get; set; }

        public int? IdBranch { get; set; }

        public bool? Enable { get; set; }

        [StringLength(250)]
        public string TransferContent { get; set; }

        public virtual Branch Branch { get; set; }
    }
}
