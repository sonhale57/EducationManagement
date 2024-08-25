using SuperbrainManagement.DTOs;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static SuperbrainManagement.MvcApplication;

namespace SuperbrainManagement.Helpers
{
    public class StudentHelper
    {
        private ModelDbContext db = new ModelDbContext();
        public List<Student> GetPotentialStudent()
        {
            var students = db.Students
                           .GroupJoin(db.Registrations,
                               student => student.Id,
                               registration => registration.IdStudent,
                               (student, registrations) => new { Student = student, Registrations = registrations })
                           .Where(x => !x.Registrations.Any() && x.Student.Enable == true)
                           .Select(x => x.Student)
                           .ToList();

            return students;
        }

        public List<Student> GetOfficialStudent()
        {
            var students = db.Students
                            .Include(x => x.User)
                            .Include(x => x.Registrations)
                            .Join(db.Registrations,
                               student => student.Id,
                               registration => registration.IdStudent,
                               (student, registration) => new { Student = student, Registration = registration })
                            .Where(x => x.Student.Enable==true && x.Student.Id == x.Registration.IdStudent)
                            .Select(x => x.Student)
                            .Distinct().ToList();
            return students;
        }

        public List<StudentViewDTO> GetOfficialStudentWithStatus(List<Student> students, bool isPotential)
        {
            var studentsDto = students.Select(x => new StudentViewDTO
            {
                Id = x.Id,
                Code = x.Code,
                DateOfBirth = x.DateOfBirth,
                Name = x.Name,
                Sex = x.Sex,
                Status = isPotential ? "Tiềm năng" : GetStatus(x.Id),
                NameOfUser = x.User.Name
            }).ToList();

            return studentsDto;
        }

        private string GetStatus(int idStudent)
        {
            var studentJoinClass = db.StudentJoinClasses.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IdStudent == idStudent);

            if (studentJoinClass == null)
            {
                var student = db.Students.FirstOrDefault(x => x.Id == idStudent);

                if (student == null)
                {
                    return "Không xác định";
                }

                else if (student.Registrations.Any(r => r.RegistrationCourses.Any(s => s.StatusJoinClass == false)))
                {
                    return "<span class='badge bg-secondary rounded-3 fw-semibold'>Chờ xét lớp</span>";
                }

                else
                {
                    if (student.Registrations.Any(r => r.RegistrationCourses.Any(s => s.StatusJoinClass == false)))
                    {
                        return "<span class='badge bg-secondary rounded-3 fw-semibold'>Chờ xét lớp</span>";
                    }
                    return "<span class='badge bg-danger rounded-3 fw-semibold'>Đã kết thúc</span>";
                }
            }

            if (studentJoinClass.Fromdate <= DateTime.Now && DateTime.Now <= studentJoinClass.Todate) { 
                return "<span class='badge bg-success rounded-3 fw-semibold'>Đang học</span>";
            }

            if (studentJoinClass.Todate < DateTime.Now)
            {
                return "<span class='badge bg-danger rounded-3 fw-semibold'>Đã kết thúc</span>";
            }

            return "";
        }
    }
}