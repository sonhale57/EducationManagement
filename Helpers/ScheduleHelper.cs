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
            List<Schedule> schedules = new List<Schedule>
            {
                new Schedule()
                {
                    IdWeek = 0,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 1,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 2,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 3,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 4,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 5,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
                },
                new Schedule()
                {
                    IdWeek = 6,
                    IdClass = idClass,
                    Active = false,
                    FromHour = DateTime.Now,
                    ToHour = DateTime.Now,
                    IdEmployee = 1,
                    IdRoom = 6
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