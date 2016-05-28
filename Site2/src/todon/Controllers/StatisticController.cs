using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using todon.Models;
using todon.ViewModels.Statistics;
using todon.ViewModels.UserTasks;
using Microsoft.AspNet.Authorization;
using System.Web.Helpers;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace todon.Controllers
{
    [Authorize]
    public class StatisticController : Controller
    {

        private ApplicationDbContext _context;

        public StatisticController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
          
            return View();
        }

        #region TaskStatisticsMonth

        public IActionResult TaskStatisticsMonth()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                StatisticsViewModel svm = new StatisticsViewModel(_context);
                return View("TaskStatisticsMonth", svm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        //public IActionResult TaskStatisticsMonthDiagram()
        //{
        //    UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
        //    if (u != null)
        //    {
        //        StatisticsViewModel svm = new StatisticsViewModel(_context);
        //        return View("TaskStatisticsMonthDiagram", svm);


        //    }
        //    return View("~/Views/Home/MessageForLogin.cshtml");
        //}

        #endregion

        #region TasksNonDate

        public IActionResult TasksNonDate()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                UserTaskListViewModel tlvm = new UserTaskListViewModel();
                tlvm.Tasks = new List<UserTaskViewModel>();
                foreach (UserTask tas in UserTask.GetTasksWithoutDone(_context))
                {
                    if (tas.UserId == u.UserId && tas.TaskType == 1 && tas.DateStart == null)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, (int)tas.UserId);
                    }
                }
                return View("TasksView", tlvm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        public IActionResult TasksOverdueNonPeriod()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                UserTaskListViewModel tlvm = new UserTaskListViewModel();
                tlvm.Tasks = new List<UserTaskViewModel>();
                foreach (UserTask tas in UserTask.GetTasksOverdueNonPeriod(_context))
                {
                    if (tas.UserId == u.UserId && tas.TaskType == 1 && tas.DateStart != null)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context,(int)tas.UserId);
                    }
                }
                tlvm.Tasks = tlvm.Tasks.OrderByDescending(obj => obj.Priority).ToList();

                return View("TasksOverdueNonPeriod", tlvm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        #endregion

        #region TaskStatisticsLastMonth

        public IActionResult TaskStatisticsLastMonth()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                StatisticsViewModel svm = new StatisticsViewModel(_context);
                return View("TaskStatisticsLastMonth", svm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        //public IActionResult TaskStatisticsLastMonthDiagram()
        //{
        //    UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
        //    if (u != null)
        //    {
        //        StatisticsViewModel svm = new StatisticsViewModel(_context);
        //        return View("TaskStatisticsLastMonthDiagram", svm);
        //    }
        //    return View("~/Views/Home/MessageForLogin.cshtml");
        //}

        #endregion

        #region TaskStatisticsWeek

        public IActionResult TaskStatisticsWeek()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                DateTime dateValue = DateTime.Parse(DateTime.Now.ToShortDateString());
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                while (dateValue.DayOfWeek != firstDayOfWeek)
                {
                    dateValue = dateValue.AddDays(-1);
                }
                //int dayWeek = DateTime.Now.Day - 7;
                //if (dayWeek <= 0)
                //    dayWeek = 0;
                StatisticsViewModel svm = new StatisticsViewModel(_context, dateValue.Day);
                return View("TaskStatisticsWeek", svm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        public IActionResult TaskStatisticsWeekDiagram()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                DateTime dateValue = DateTime.Parse(DateTime.Now.ToShortDateString());
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                while (dateValue.DayOfWeek != firstDayOfWeek)
                {
                    dateValue = dateValue.AddDays(-1);
                }
                //int dayWeek = DateTime.Now.Day - 7;
                //if (dayWeek <= 0)
                //    dayWeek = 0;
                StatisticsViewModel svm = new StatisticsViewModel(_context, dateValue.Day);

                return View("TaskStatisticsWeekDiagram", svm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        #endregion

        # region TaskStatisticsGetNamesByDayNum

        public IActionResult TaskStatisticsGetNamesByDayNum(int id)
        {
            StatisticsViewModel svm = new StatisticsViewModel();
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, id);
            foreach (Done d in Done.GetDoneByDate(date, _context))
            {
                UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == d.TaskId);
                svm.DoneTasksNames.Add(d.TaskId, t.Name);
            }
            return View("TaskStatisticsGetNamesByDayNum", svm);
        }

        #endregion

        public IActionResult TaskStatistics()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                return View("TaskStatistics");
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

    }
}
