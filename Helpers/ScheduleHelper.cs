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
    }
}