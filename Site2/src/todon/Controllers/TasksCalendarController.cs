using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using todon.Models;
using System;
using todon.ViewModels.UserTasks;
using System.Collections.Generic;
using System.Globalization;
using todon.ViewModels.WeekTasks;
using Microsoft.AspNet.Authorization;

namespace todon.Controllers
{
    [Authorize]
    public class TasksCalendarController : Controller
    {
        private ApplicationDbContext _context;
       // UserProfile user; 
        public TasksCalendarController(ApplicationDbContext context)
        {
            _context = context;
           // user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
        }

        #region TaskCalendar

        public IActionResult TaskCalendar()
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                var date = Helper.ConvertDateStartToString(DateTime.Now.ToString());
                return TaskCalendarByDate(date);
            }
            return HttpNotFound();
        }

        public IActionResult TaskCalendarByDate(string taskDate)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                UserTaskListViewModel tlvm = new UserTaskListViewModel();
                tlvm.Tasks = new List<UserTaskViewModel>();
                int date_start_hour = 0;

                DateTime dateValue = DateTime.Parse(taskDate);//DateTime.ParseExact(taskDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                tlvm.Date = dateValue;
                foreach (UserTask tas in UserTask.GetTasksByDate(true, dateValue,_context))
                {
                    var task_done = UserTask.GetTasksDoneByDate(true, dateValue,_context).Find(x => x.TaskId == tas.TaskId);
                    if (task_done == null)
                    {
                        // формирование списка дел с датой
                        var date_Start = Helper.ConvertDateStartToString(tas.DateStart.ToString());
                        if (tas.DateStart != null)
                        {
                            date_start_hour = Int32.Parse(Helper.ConvertDateStartHourToInt(tas.DateStart.ToString()));
                        }
                        if (tas.UserId == u.UserId && date_Start == taskDate)
                        {
                            UserTaskViewModel tvmt = new UserTaskViewModel(_context,tas);
                            if (date_start_hour != 0)
                            {
                                tvmt.TimeHour = date_start_hour;
                            }
                            tlvm.CalendarTasks.Add(tvmt);
                        }
                        if (tas.UserId == u.UserId && date_Start == "" || date_Start == null)
                        {
                            UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);

                            tvmt.TimeHour = Int32.Parse(Helper.ConvertPeriodToTime(tas.Period));
                            tlvm.Date = DateTime.Parse(taskDate);
                            tlvm.CalendarTasks.Add(tvmt);
                        }
                    }
                }

                foreach (UserTask tas in UserTask.GetTasksWithoutDoneByDate(false, dateValue,_context))
                {
                    bool useDate = true;
                    if (tas.DateStart == null) useDate = false;
                    else if (tas.DateStart.Value.TimeOfDay == TimeSpan.Zero) useDate = false;

                    //формирование списка дел без даты
                    if (tas.UserId == u.UserId && !useDate)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);

                        tlvm.Tasks.Add(tvmt);
                    }
                }

                foreach (UserTask tas in UserTask.GetTasksDoneByDate(false, dateValue,_context))
                {
                    //формирование списка дел без даты
                    if (tas.UserId == u.UserId)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);

                        tlvm.DoneTasks.Add(tvmt);
                    }
                }

                tlvm.Tasks = tlvm.Tasks.OrderByDescending(obj => obj.Priority).ToList();
                tlvm.Tasks = tlvm.Tasks.OrderBy(obj => obj.DateStart).ToList();

                return View("TaskCalendar", tlvm);
            }
            return HttpNotFound();
        }

        #endregion     

        #region TaskCalendarWeek

        public IActionResult TaskCalendarWeek(string taskDate)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            WeekTaskViewModel tlvm = new WeekTaskViewModel();
           // int date_start_hour = 0;
            DateTime dateValue = DateTime.Parse(taskDate);// DateTime.ParseExact(taskDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            //tlvm.CurrentDate = dateValue;
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            while (dateValue.DayOfWeek != firstDayOfWeek)
            {
                dateValue = dateValue.AddDays(-1);
            }
            tlvm.CurrentDate = dateValue;
            tlvm.StartDate = dateValue;
            tlvm.CalendarTasksMonday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue,_context));
            tlvm.CalendarTasksThuesday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+1), _context));
            tlvm.CalendarTasksWednesday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+2), _context));
            tlvm.CalendarTasksThursday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+3), _context));
            tlvm.CalendarTasksFriday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+4), _context));
            tlvm.CalendarTasksSaturday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+5), _context));
            tlvm.CalendarTasksSunday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+6), _context));

            tlvm.Tasks = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneWeekByDate(false, dateValue, dateValue.AddDays(7).AddSeconds(-1), _context));
            tlvm.DoneTasks = ConvertTaskList2TaskViewModelList(UserTask.GetTasksDoneWeekByDate(false, dateValue, dateValue.AddDays(7).AddSeconds(-1), _context));


            return View("TaskCalendarForWeekByDate", tlvm);
            //tlvm.Date = dateValue;



        }

        public IActionResult TaskCalendarForWeekByDate(string taskDate)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                UserTaskListViewModel tlvm = new UserTaskListViewModel();
                UserTaskListViewModel tlvm2 = new UserTaskListViewModel();
                tlvm.Tasks = new List<UserTaskViewModel>();
                int date_start_hour = 0;
                DateTime dateValue = DateTime.ParseExact(taskDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                foreach (UserTask tas in UserTask.GetTasksByDate(true, dateValue,_context))
                {
                    var task_done = UserTask.GetTasksDoneByDate(true, dateValue,_context).Find(x => x.TaskId == tas.TaskId);
                    if (task_done == null)
                    {
                        // формирование списка дел с датой
                        var date_Start = Helper.ConvertDateStartToString(tas.DateStart.ToString());
                        if (tas.DateStart != null)
                        {
                            date_start_hour = Int32.Parse(Helper.ConvertDateStartHourToInt(tas.DateStart.ToString()));
                        }
                        if (tas.UserId == u.UserId && date_Start == taskDate)
                        {
                            UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
                            if (date_start_hour != 0)
                            {
                                tvmt.TimeHour = date_start_hour;
                            }
                            tlvm.CalendarTasks.Add(tvmt);
                        }
                        if (tas.UserId == u.UserId && date_Start == "" || date_Start == null)
                        {
                            UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);

                            tvmt.TimeHour = Int32.Parse(Helper.ConvertPeriodToTime(tas.Period));

                            tlvm.CalendarTasks.Add(tvmt);
                        }
                    }
                }

                foreach (UserTask tas in UserTask.GetTasksWithoutDoneByDate(false, dateValue,_context))
                {
                    bool useDate = true;
                    if (tas.DateStart == null) useDate = false;
                    else if (tas.DateStart.Value.TimeOfDay == TimeSpan.Zero) useDate = false;

                    //формирование списка дел без даты
                    if (tas.UserId == u.UserId && !useDate)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);

                        tlvm.Tasks.Add(tvmt);
                    }
                }

                foreach (UserTask tas in UserTask.GetTasksDoneByDate(false, dateValue,_context))
                {
                    //формирование списка дел без даты
                    if (tas.UserId == u.UserId)
                    {
                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);

                        tlvm.DoneTasks.Add(tvmt);
                    }
                }

                tlvm.Tasks = tlvm.Tasks.OrderByDescending(obj => obj.Priority).ToList();
                tlvm.Tasks = tlvm.Tasks.OrderBy(obj => obj.DateStart).ToList();
                tlvm.Date = dateValue;

                return View("TaskCalendarForWeekByDate", tlvm);
            }
            return View("~/Views/Home/MessageForLogin.cshtml");
        }

        private List<UserTaskViewModel> ConvertTaskList2TaskViewModelList(List<UserTask> tasklist)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            List<UserTaskViewModel> result = new List<UserTaskViewModel>();
            foreach (UserTask tas in tasklist)
            {
                int date_start_hour = 0;
                UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);
                if (tas.DateStart != null)
                {
                    date_start_hour = Int32.Parse(Helper.ConvertDateStartHourToInt(tas.DateStart.ToString()));
                }
                tvmt.TaskId = tas.TaskId;
                if (tas.Name.Length > 10)
                {
                    tvmt.Name = tas.Name.Substring(0, 9) + "...";
                }
                else
                    tvmt.Name = tas.Name;
                tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
                //tvmt.Picture = tas.Picture;
                tvmt.DurationStr = Helper.ConvertMinsToDuration(tas.Duration);
                tvmt.DateStart = tas.DateStart;
                tvmt.DateEnd = tas.DateEnd;
                tvmt.CategoryPicture = tas.Picture;
                if (date_start_hour != 0)
                {
                    tvmt.TimeHour = date_start_hour;
                }
                result.Add(tvmt);
            }
            return result;
        }

        #endregion

        #region TaskMove

        public IActionResult TaskMove(int id)
        {
           
            UserTaskViewModel cvm = new UserTaskViewModel();
            cvm.TaskId = id;

            return View("TaskMove", cvm);
        }

        public IActionResult TaskMovePeriod(int id)
        {
           
            UserTaskViewModel cvm = new UserTaskViewModel();
            cvm.TaskId = id;

            return View("TaskMovePeriod", cvm);
        }

        public IActionResult TaskMoveNonTime(int id)
        {
           
            UserTaskViewModel cvm = new UserTaskViewModel();
            cvm.TaskId = id;
            return View("TaskMoveNonTime", cvm);
        }

        public IActionResult SaveTaskMove(int id)
        {
            UserTask cat = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
            string moveValue = Request.Form["moveValue"];
            if (moveValue == "0")
                moveValue = Request.Form["userMoveValue"];

            Int32? moveDuration = Helper.ConvertDurationToInt32(moveValue);
            if (cat.DateStart != null)
                cat.DateStart = cat.DateStart.Value.AddMinutes((double)moveDuration);
            if (cat.DateEnd != null)
                cat.DateEnd = cat.DateEnd.Value.AddMinutes((double)moveDuration);
            var task = UserTask.EditTask(cat,_context);
            return TaskCalendar();
        }

        public IActionResult SaveTaskMovePeriod(int id)
        {
            UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
            string moveValue = Request.Form["moveValue"];
            if (moveValue == "0")
                moveValue = Request.Form["userMoveValue"];
            t.DateStart = Helper.ConvertPeriodToDateStart(t.Period);
            Int32? moveDuration = Helper.ConvertDurationToInt32(moveValue);
            if (t.DateStart != null)
                t.DateStart = t.DateStart.Value.AddMinutes((double)moveDuration);
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            t.Period = "";
            Done d = new Done()
            {
                TaskId = id,
                UserId = u.UserId,
                DateDone = DateTime.Now
            };
            int done_id = Done.AddDone(d,_context).DoneId;
            int task_id = UserTask.AddTask(t,_context).TaskId;
            return TaskCalendar();
        }

        public IActionResult SaveTaskMoveNonTime(int id)
        {
            UserTask cat = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
            string moveValue = Request.Form["moveValue"];
            if (moveValue == "0")
                moveValue = Request.Form["userMoveValue"];

            Int32? moveDuration = Helper.ConvertDayToInt32(moveValue);
            if (cat.DateActivate != null)
                cat.DateActivate = cat.DateActivate.Value.AddDays((double)moveDuration);
            else
                cat.DateActivate = DateTime.Now.AddDays((double)moveDuration);
            //if (cat.Date_end == null)
            //    cat.Date_end = cat.Date_end.Value.AddDays((double)moveDuration);
            int tas_id = UserTask.EditTask(cat,_context).TaskId;
            return TaskCalendar();
        }

        #endregion

        #region MarkTask

        public IActionResult MarkTaskDone(int id)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            Done d = new Done()
            {
                TaskId = id,
                UserId = u.UserId,
                DateDone = DateTime.Now
            };
            int done_id = Done.AddDone(d,_context).DoneId;
            return TaskCalendar();
        }

        public IActionResult MarkTaskWeekDone(int id)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            Done d = new Done()
            {
                TaskId = id,
                UserId = u.UserId,
                DateDone = DateTime.Now
            };
            int done_id = Done.AddDone(d,_context).DoneId;
            var date = Helper.ConvertDateStartToString(DateTime.Now.ToString());
            return TaskCalendarWeek(date);
        }

        public IActionResult MarkTaskUndone(int id)
        {
            UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());

            if (Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId) != null)
            {
                Done task = Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId).Last();
                if (task != null)
                {
                    Done.DeleteDone(task,_context);
                }
            }
            return TaskCalendar();

        }

        public IActionResult MarkTaskWeekUndone(int id)
        {
            UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());

            if (Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId) != null)
            {
                Done task = Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId).Last();
                if (task != null)
                {
                    Done.DeleteDone(task,_context);
                }
            }
            var date = Helper.ConvertDateStartToString(DateTime.Now.ToString());
            return TaskCalendarWeek(date);

        }

        public IActionResult TaskCalendarByDateChangeFormat(string taskDate)
        {
            DateTime date;
            if (String.IsNullOrEmpty(taskDate))
            {
                taskDate = DateTime.Now.ToString();
                date = DateTime.Parse(taskDate);
            }
            else
                date = DateTime.ParseExact(taskDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var dateShort = Helper.ConvertDateStartToString(date.ToString());
            return TaskCalendarByDate(dateShort);

        }

        #endregion

    }
}










//[Authorize]
//public class TasksCalendarController : Controller
//{
//    private ApplicationDbContext _context;
//    // UserProfile user; 
//    public TasksCalendarController(ApplicationDbContext context)
//    {
//        _context = context;
//        // user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//    }

//    public IActionResult TaskCalendar()
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        if (u != null)
//        {
//            var date = UserTask.ConvertDateStartToString(DateTime.Now.ToString());
//            return TaskCalendarByDate(date);
//        }
//        return HttpNotFound();
//    }

//    public IActionResult TaskCalendarByDate(string taskDate)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        if (u != null)
//        {
//            UserTaskListViewModel tlvm = new UserTaskListViewModel();
//            tlvm.Tasks = new List<UserTaskViewModel>();
//            int date_start_hour = 0;

//            DateTime dateValue = DateTime.Parse(taskDate);//DateTime.ParseExact(taskDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
//            tlvm.Date = dateValue;
//            foreach (UserTask tas in UserTask.GetTasksByDate(true, dateValue, _context))
//            {
//                var task_done = UserTask.GetTasksDoneByDate(true, dateValue, _context).Find(x => x.TaskId == tas.TaskId);
//                if (task_done == null)
//                {
//                    // формирование списка дел с датой
//                    var date_Start = UserTask.ConvertDateStartToString(tas.DateStart.ToString());
//                    if (tas.DateStart != null)
//                    {
//                        date_start_hour = Int32.Parse(UserTask.ConvertDateStartHourToInt(tas.DateStart.ToString()));
//                    }
//                    if (tas.UserId == u.UserId && date_Start == taskDate)
//                    {
//                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
//                        //tvmt.TaskId = tas.TaskId;
//                        //tvmt.Name = tas.Name;
//                        ////tvm.Category_id = u.category_id;
//                        //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                        //tvmt.Picture = tas.Picture;
//                        //tvmt.DurationStr = UserTask.ConvertMinsToDuration(tas.Duration);
//                        //tvmt.DateStart = tas.DateStart;
//                        //tvmt.DateEnd = tas.DateEnd;
//                        //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId, _context).Picture;
//                        if (date_start_hour != 0)
//                        {
//                            tvmt.TimeHour = date_start_hour;
//                        }
//                        tlvm.CalendarTasks.Add(tvmt);
//                    }
//                    if (tas.UserId == u.UserId && date_Start == "" || date_Start == null)
//                    {
//                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
//                        //tvmt.TaskId = tas.TaskId;
//                        //tvmt.Name = tas.Name;
//                        ////tvm.Category_id = u.category_id;
//                        //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                        //tvmt.Picture = tas.Picture;
//                        //// tvmt.Duration = Task.ConvertMinsToDuration(tas.Duration);
//                        //tvmt.DateStart = tas.DateStart;
//                        //tvmt.DateEnd = tas.DateEnd;
//                        //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;

//                        tvmt.TimeHour = Int32.Parse(UserTask.ConvertPeriodToTime(tas.Period));
//                        tlvm.Date = DateTime.Parse(taskDate);
//                        tlvm.CalendarTasks.Add(tvmt);
//                    }
//                }
//            }

//            foreach (UserTask tas in UserTask.GetTasksWithoutDoneByDate(false, dateValue, _context))
//            {
//                bool useDate = true;
//                if (tas.DateStart == null) useDate = false;
//                else if (tas.DateStart.Value.TimeOfDay == TimeSpan.Zero) useDate = false;

//                //формирование списка дел без даты
//                if (tas.UserId == u.UserId && !useDate)
//                {
//                    UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
//                    //tvmt.TaskId = tas.TaskId;
//                    //tvmt.Name = tas.Name;
//                    ////tvm.Category_id = u.category_id;
//                    //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                    //tvmt.Picture = tas.Picture;
//                    //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;
//                    ////tvmt.Date_start = tas.Date_start;

//                    tlvm.Tasks.Add(tvmt);
//                }
//            }

//            foreach (UserTask tas in UserTask.GetTasksDoneByDate(false, dateValue, _context))
//            {
//                //формирование списка дел без даты
//                if (tas.UserId == u.UserId)
//                {
//                    UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
//                    //tvmt.TaskId = tas.TaskId;
//                    //tvmt.Name = tas.Name;
//                    ////tvm.Category_id = u.category_id;
//                    //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                    //tvmt.Picture = tas.Picture;
//                    //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;
//                    ////tvmt.Date_start = tas.Date_start;

//                    tlvm.DoneTasks.Add(tvmt);
//                }
//            }

//            tlvm.Tasks = tlvm.Tasks.OrderByDescending(obj => obj.Priority).ToList();
//            tlvm.Tasks = tlvm.Tasks.OrderBy(obj => obj.DateStart).ToList();

//            return View("TaskCalendar", tlvm);
//        }
//        return HttpNotFound();
//    }

//    private List<UserTaskViewModel> ConvertTaskList2TaskViewModelList(List<UserTask> tasklist)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        List<UserTaskViewModel> result = new List<UserTaskViewModel>();
//        foreach (UserTask tas in tasklist)
//        {
//            int date_start_hour = 0;
//            UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);
//            if (tas.DateStart != null)
//            {
//                date_start_hour = Int32.Parse(UserTask.ConvertDateStartHourToInt(tas.DateStart.ToString()));
//            }
//            tvmt.TaskId = tas.TaskId;
//            if (tas.Name.Length > 10)
//            {
//                tvmt.Name = tas.Name.Substring(0, 9) + "...";
//            }
//            else
//                tvmt.Name = tas.Name;
//            tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//            tvmt.Picture = tas.Picture;
//            tvmt.DurationStr = UserTask.ConvertMinsToDuration(tas.Duration);
//            tvmt.DateStart = tas.DateStart;
//            tvmt.DateEnd = tas.DateEnd;
//            tvmt.CategoryPicture = tas.Picture;
//            if (date_start_hour != 0)
//            {
//                tvmt.TimeHour = date_start_hour;
//            }
//            result.Add(tvmt);
//        }
//        return result;
//    }

//    public IActionResult TaskCalendarWeek(string taskDate)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        WeekTaskViewModel tlvm = new WeekTaskViewModel();
//        // int date_start_hour = 0;
//        DateTime dateValue = DateTime.Parse(taskDate);// DateTime.ParseExact(taskDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

//        //tlvm.CurrentDate = dateValue;
//        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
//        while (dateValue.DayOfWeek != firstDayOfWeek)
//        {
//            dateValue = dateValue.AddDays(-1);
//        }
//        tlvm.CurrentDate = dateValue;
//        tlvm.StartDate = dateValue;
//        tlvm.CalendarTasksMonday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue, _context));
//        tlvm.CalendarTasksThuesday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+1), _context));
//        tlvm.CalendarTasksWednesday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+2), _context));
//        tlvm.CalendarTasksThursday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+3), _context));
//        tlvm.CalendarTasksFriday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+4), _context));
//        tlvm.CalendarTasksSaturday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+5), _context));
//        tlvm.CalendarTasksSunday = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneByDate(true, dateValue.AddDays(+6), _context));

//        tlvm.Tasks = ConvertTaskList2TaskViewModelList(UserTask.GetTasksWithoutDoneWeekByDate(false, dateValue, dateValue.AddDays(7).AddSeconds(-1), _context));
//        tlvm.DoneTasks = ConvertTaskList2TaskViewModelList(UserTask.GetTasksDoneWeekByDate(false, dateValue, dateValue.AddDays(7).AddSeconds(-1), _context));


//        return View("TaskCalendarForWeekByDate", tlvm);
//        //tlvm.Date = dateValue;



//    }

//    public IActionResult TaskCalendarForWeekByDate(string taskDate)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        if (u != null)
//        {
//            UserTaskListViewModel tlvm = new UserTaskListViewModel();
//            UserTaskListViewModel tlvm2 = new UserTaskListViewModel();
//            tlvm.Tasks = new List<UserTaskViewModel>();
//            int date_start_hour = 0;
//            DateTime dateValue = DateTime.ParseExact(taskDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

//            foreach (UserTask tas in UserTask.GetTasksByDate(true, dateValue, _context))
//            {
//                var task_done = UserTask.GetTasksDoneByDate(true, dateValue, _context).Find(x => x.TaskId == tas.TaskId);
//                if (task_done == null)
//                {
//                    // формирование списка дел с датой
//                    var date_Start = UserTask.ConvertDateStartToString(tas.DateStart.ToString());
//                    if (tas.DateStart != null)
//                    {
//                        date_start_hour = Int32.Parse(UserTask.ConvertDateStartHourToInt(tas.DateStart.ToString()));
//                    }
//                    if (tas.UserId == u.UserId && date_Start == taskDate)
//                    {
//                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, tas);
//                        //tvmt.TaskId = tas.TaskId;
//                        //tvmt.Name = tas.Name;
//                        ////tvm.Category_id = u.category_id;
//                        //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                        //tvmt.Picture = tas.Picture;
//                        //tvmt.DurationStr = UserTask.ConvertMinsToDuration(tas.Duration);
//                        //tvmt.DateStart = tas.DateStart;
//                        //tvmt.DateEnd = tas.DateEnd;
//                        //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;
//                        if (date_start_hour != 0)
//                        {
//                            tvmt.TimeHour = date_start_hour;
//                        }
//                        tlvm.CalendarTasks.Add(tvmt);
//                    }
//                    if (tas.UserId == u.UserId && date_Start == "" || date_Start == null)
//                    {
//                        UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);
//                        //tvmt.TaskId = tas.TaskId;
//                        //tvmt.Name = tas.Name;
//                        ////tvm.Category_id = u.category_id;
//                        //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                        //tvmt.Picture = tas.Picture;
//                        //// tvmt.Duration = Task.ConvertMinsToDuration(tas.Duration);
//                        //tvmt.DateStart = tas.DateStart;
//                        //tvmt.DateEnd = tas.DateEnd;
//                        //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;

//                        tvmt.TimeHour = Int32.Parse(UserTask.ConvertPeriodToTime(tas.Period));

//                        tlvm.CalendarTasks.Add(tvmt);
//                    }
//                }
//            }

//            foreach (UserTask tas in UserTask.GetTasksWithoutDoneByDate(false, dateValue, _context))
//            {
//                bool useDate = true;
//                if (tas.DateStart == null) useDate = false;
//                else if (tas.DateStart.Value.TimeOfDay == TimeSpan.Zero) useDate = false;

//                //формирование списка дел без даты
//                if (tas.UserId == u.UserId && !useDate)
//                {
//                    UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);
//                    //tvmt.TaskId = tas.TaskId;
//                    //tvmt.Name = tas.Name;
//                    ////tvm.Category_id = u.category_id;
//                    //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                    //tvmt.Picture = tas.Picture;
//                    //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;
//                    ////tvmt.Date_start = tas.Date_start;

//                    tlvm.Tasks.Add(tvmt);
//                }
//            }

//            foreach (UserTask tas in UserTask.GetTasksDoneByDate(false, dateValue, _context))
//            {
//                //формирование списка дел без даты
//                if (tas.UserId == u.UserId)
//                {
//                    UserTaskViewModel tvmt = new UserTaskViewModel(_context, u.UserId);
//                    //tvmt.TaskId = tas.TaskId;
//                    //tvmt.Name = tas.Name;
//                    ////tvm.Category_id = u.category_id;
//                    //tvmt.Priority = (tas.Priority == null) ? 0 : (int)tas.Priority;
//                    //tvmt.Picture = tas.Picture;
//                    //tvmt.CategoryPicture = Category.GetCategoryById(tas.CategoryId,_context).Picture;
//                    ////tvmt.Date_start = tas.Date_start;

//                    tlvm.DoneTasks.Add(tvmt);
//                }
//            }

//            tlvm.Tasks = tlvm.Tasks.OrderByDescending(obj => obj.Priority).ToList();
//            tlvm.Tasks = tlvm.Tasks.OrderBy(obj => obj.DateStart).ToList();
//            tlvm.Date = dateValue;

//            return View("TaskCalendarForWeekByDate", tlvm);
//        }
//        return View("~/Views/Home/MessageForLogin.cshtml");
//    }

//    public IActionResult TaskCalendarByDateChangeFormat(string taskDate)
//    {
//        DateTime date;
//        if (taskDate == "")
//        {
//            taskDate = DateTime.Now.ToString();
//            date = DateTime.Parse(taskDate);
//        }
//        else
//            date = DateTime.ParseExact(taskDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
//        var dateShort = UserTask.ConvertDateStartToString(date.ToString());
//        return TaskCalendarByDate(dateShort);

//    }
//    //-------------------------------------------------
//    //--------------------------------
//    public IActionResult TaskMove(int id)
//    {

//        UserTaskViewModel cvm = new UserTaskViewModel();
//        cvm.TaskId = id;

//        return View("TaskMove", cvm);
//    }

//    public IActionResult TaskMovePeriod(int id)
//    {

//        UserTaskViewModel cvm = new UserTaskViewModel();
//        cvm.TaskId = id;

//        return View("TaskMovePeriod", cvm);
//    }

//    public IActionResult TaskMoveNonTime(int id)
//    {

//        UserTaskViewModel cvm = new UserTaskViewModel();
//        cvm.TaskId = id;
//        return View("TaskMoveNonTime", cvm);
//    }

//    public IActionResult SaveTaskMove(int id)
//    {
//        UserTask cat = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
//        string moveValue = Request.Form["moveValue"];
//        if (moveValue == "0")
//            moveValue = Request.Form["userMoveValue"];

//        Int32? moveDuration = UserTask.ConvertDurationToInt32(moveValue);
//        if (cat.DateStart != null)
//            cat.DateStart = cat.DateStart.Value.AddMinutes((double)moveDuration);
//        if (cat.DateEnd != null)
//            cat.DateEnd = cat.DateEnd.Value.AddMinutes((double)moveDuration);
//        var task = UserTask.EditTask(cat, _context);
//        return TaskCalendar();
//    }

//    public IActionResult SaveTaskMovePeriod(int id)
//    {
//        UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
//        string moveValue = Request.Form["moveValue"];
//        if (moveValue == "0")
//            moveValue = Request.Form["userMoveValue"];
//        t.DateStart = UserTask.ConvertPeriodToDateStart(t.Period);
//        Int32? moveDuration = UserTask.ConvertDurationToInt32(moveValue);
//        if (t.DateStart != null)
//            t.DateStart = t.DateStart.Value.AddMinutes((double)moveDuration);
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        t.Period = "";
//        Done d = new Done()
//        {
//            TaskId = id,
//            UserId = u.UserId,
//            DateDone = DateTime.Now
//        };
//        int done_id = Done.AddDone(d, _context).DoneId;
//        int task_id = UserTask.AddTask(t, _context).TaskId;
//        return TaskCalendar();
//    }

//    public IActionResult SaveTaskMoveNonTime(int id)
//    {
//        UserTask cat = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
//        string moveValue = Request.Form["moveValue"];
//        if (moveValue == "0")
//            moveValue = Request.Form["userMoveValue"];

//        Int32? moveDuration = UserTask.ConvertDayToInt32(moveValue);
//        if (cat.DateActivate != null)
//            cat.DateActivate = cat.DateActivate.Value.AddDays((double)moveDuration);
//        else
//            cat.DateActivate = DateTime.Now.AddDays((double)moveDuration);
//        //if (cat.Date_end == null)
//        //    cat.Date_end = cat.Date_end.Value.AddDays((double)moveDuration);
//        int tas_id = UserTask.EditTask(cat, _context).TaskId;
//        return TaskCalendar();
//    }

//    public IActionResult MarkTaskDone(int id)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        Done d = new Done()
//        {
//            TaskId = id,
//            UserId = u.UserId,
//            DateDone = DateTime.Now
//        };
//        int done_id = Done.AddDone(d, _context).DoneId;
//        return TaskCalendar();
//    }

//    public IActionResult MarkTaskWeekDone(int id)
//    {
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//        Done d = new Done()
//        {
//            TaskId = id,
//            UserId = u.UserId,
//            DateDone = DateTime.Now
//        };
//        int done_id = Done.AddDone(d, _context).DoneId;
//        var date = UserTask.ConvertDateStartToString(DateTime.Now.ToString());
//        return TaskCalendarWeek(date);
//    }

//    public IActionResult MarkTaskUndone(int id)
//    {
//        UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());

//        if (Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId) != null)
//        {
//            Done task = Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId).Last();
//            if (task != null)
//            {
//                Done.DeleteDone(task, _context);
//            }
//        }
//        return TaskCalendar();

//    }

//    public IActionResult MarkTaskWeekUndone(int id)
//    {
//        UserTask t = UserTask.GetTasks(_context).Find(x => x.TaskId == id);
//        UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());

//        if (Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId) != null)
//        {
//            Done task = Done.GetTodayDone(_context).Where(x => x.TaskId == id && x.UserId == u.UserId).Last();
//            if (task != null)
//            {
//                Done.DeleteDone(task, _context);
//            }
//        }
//        var date = UserTask.ConvertDateStartToString(DateTime.Now.ToString());
//        return TaskCalendarWeek(date);

//    }
//}
//}
