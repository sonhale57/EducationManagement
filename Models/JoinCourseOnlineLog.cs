namespace SuperbrainManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JoinCourseOnlineLog")]
    public partial class JoinCourseOnlineLog
    {
        public int Id { get; set; }

        public int? IdUser { get; set; }

        public int? IdLesson { get; set; }

        public DateTime? DateCreate { get; set; }

        public bool? StatusComplete { get; set; }

        public virtual LessonCourseOnline LessonCourseOnline { get; set; }

        public virtual User User { get; set; }
    }
}
