using System;

namespace SuperbrainManagement.DTOs
{
    public class StudentViewDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string NameOfUser { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Sex { get; set; }

        public string Status { get; set; }
    }
}