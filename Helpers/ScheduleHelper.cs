using SuperbrainManagement.Controllers;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperbrainManagement.Helpers
{
    public class ScheduleHelper
    {
        private ModelDbContext db = new ModelDbContext();

        public List<Schedule> GetScheduleDefault(int idClass)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var cla = db.Classes.FirstOrDefault(c => c.Id == idClass);
            var room = db.Rooms.FirstOrDefault(r => r.IdBranch == idbranch);
            var emp = db.Employees.FirstOrDefault(e => e.IdBranch == idbranch && e.Enable == true && e.IsOfficial == true);
            List<Schedule> schedules = new List<Schedule>
            {
                new Schedule()
                {
                    IdWeek = 0,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 1,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 2,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 3,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 4,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 5,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
                new Schedule()
                {
                    IdWeek = 6,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = (emp==null?0:emp.Id),
                    IdRoom =(room==null?0:room.Id),
                },
            };
            return schedules;
        }

        public string GetDayName(int dayNumber)
        {
            switch (dayNumber)
            {
                case 0:
                    return "Chủ Nhật";
                case 1:
                    return "Thứ 2";
                case 2:
                    return "Thứ 3";
                case 3:
                    return "Thứ 4";
                case 4:
                    return "Thứ 5";
                case 5:
                    return "Thứ 6";
                case 6:
                    return "Thứ 7";
                default:
                    return "Không hợp lệ";
            }
        }
        public string GetRoomName(int IdClass,int IdWeek)
        {
            var schedules = db.Schedules.FirstOrDefault(x=>x.IdClass==IdClass&&x.IdWeek==IdWeek);
            if(schedules == null)
            {
                return "Không tìm thấy phòng";
            }
            else
            {
                int idroom = (int)schedules.IdRoom;
                var room = db.Rooms.Find(idroom).Name;
                return room;
            }
        }
        public double GetHourQuantity(DateTime fromDate, DateTime toDate)
        {
            TimeSpan timeQuantity = toDate - fromDate;

            return timeQuantity.Duration().TotalHours;
        }

        public string GetTimeSlot(DateTime fromHour, DateTime toHour)
        {

            return fromHour.ToString("hh:mm tt") + " - " + toHour.ToString("hh:mm tt");
        }

        public string ConvertEnglishDayToVietnamese(string englishDay)
        {
            Dictionary<string, string> englishToVietnameseDays = new Dictionary<string, string>()
            {
               { "Monday", "Thứ 2" },
                { "Tuesday", "Thứ 3" },
                { "Wednesday", "Thứ 4" },
                { "Thursday", "Thứ 5" },
                { "Friday", "Thứ 6" },
                { "Saturday", "Thứ 7" },
                { "Sunday", "Chủ nhật" }
            };
            if (englishToVietnameseDays.ContainsKey(englishDay))
            {
                return englishToVietnameseDays[englishDay];
            }
            else
            {
                return "Invalid Vietnamese day";
            }
        }
    }
}