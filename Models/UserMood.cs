namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserMood")]
    public partial class UserMood
    {
        public int Id { get; set; }

        public int? User_id { get; set; }

        [StringLength(500)]
        public string Mood_key { get; set; }

        [StringLength(500)]
        public string Mood_name { get; set; }

        public DateTime? Time_stamps { get; set; }
    }
}
