using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todon.ViewModels.UserTasks
{
    public class UserTaskListViewModel
    {
        public List<UserTaskViewModel> CalendarTasks { get; set; }
        public List<UserTaskViewModel> Tasks { get; set; }
        public List<UserTaskViewModel> DoneTasks { get; set; }

        public DateTime Date { get; set; }

        public UserTaskListViewModel()
        {
            CalendarTasks = new List<UserTaskViewModel>();
            Tasks = new List<UserTaskViewModel>();
            DoneTasks = new List<UserTaskViewModel>();
        }
    }
}
