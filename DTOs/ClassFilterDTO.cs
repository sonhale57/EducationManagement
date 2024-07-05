using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperbrainManagement.DTOs
{
    public class ClassFilterDTO
    {
        public int StudentID { get; set; }
        public string StudentName { get;set; }
        public string CourseName { get;set; }
        public List<Session> Sessions { get;set; }
    }

    public class Session
    {
        public string Date { get; set; }

        public string DayOfWeek { get; set; }

        public bool? IsCheckedIn { get; set; }
    }
}