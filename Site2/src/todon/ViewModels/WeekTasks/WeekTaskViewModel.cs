using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todon.ViewModels.UserTasks;

namespace todon.ViewModels.WeekTasks
{
    public class WeekTaskViewModel
    {
        public List<UserTaskViewModel> CalendarTasksMonday { get; set; }
        public List<UserTaskViewModel> CalendarTasksThuesday { get; set; }
        public List<UserTaskViewModel> CalendarTasksWednesday { get; set; }
        public List<UserTaskViewModel> CalendarTasksThursday { get; set; }
        public List<UserTaskViewModel> CalendarTasksFriday { get; set; }
        public List<UserTaskViewModel> CalendarTasksSaturday { get; set; }
        public List<UserTaskViewModel> CalendarTasksSunday { get; set; }
        public List<UserTaskViewModel> Tasks { get; set; }
        public List<UserTaskViewModel> DoneTasks { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CurrentDate { get; set; }

        public WeekTaskViewModel()
        {
            CalendarTasksMonday = new List<UserTaskViewModel>();
            CalendarTasksThuesday = new List<UserTaskViewModel>();
            CalendarTasksWednesday = new List<UserTaskViewModel>();
            CalendarTasksThursday = new List<UserTaskViewModel>();
            CalendarTasksFriday = new List<UserTaskViewModel>();
            CalendarTasksSaturday = new List<UserTaskViewModel>();
            CalendarTasksSunday = new List<UserTaskViewModel>();
            Tasks = new List<UserTaskViewModel>();
            DoneTasks = new List<UserTaskViewModel>();
        }
    }
}
