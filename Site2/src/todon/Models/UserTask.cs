using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace todon.Models
{
    public class UserTask
    {
        [Key]
        public Int32 TaskId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя задачи!")]
        public String Name { get; set; }
        public String Description { get; set; }
        public String Picture { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите категорию!")]
        public Int32 CategoryId { get; set; }
        public Int32? ParentId { get; set; }
        public Int32? UserId { get; set; }
        public Int32? Priority { get; set; }
        public DateTime? Date_create { get; set; }
        public Byte TaskType { get; set; }
        public String Period { get; set; }       
        public DateTime? DateStart { get; set; }       
        public Int32? Duration { get; set; }        
        public DateTime? DateEnd { get; set; }
        public String Place { get; set; }
        public DateTime? DateActivate { get; set; }

        #region CRU User
        public static UserTask AddTask(UserTask t, ApplicationDbContext _context)
        {
            _context.UserTask.Add(t);//?
            _context.SaveChanges();
            return t;
        }

        public static List<UserTask> GetTasks(ApplicationDbContext _context)
        {
            List<UserTask> tasks = _context.UserTask.ToList<UserTask>();

            return tasks;
        }

        public static UserTask EditTask(UserTask t, ApplicationDbContext _context)
        {
            Int32 taskId = t.TaskId;
            var update = _context.UserTask.Update(t);//?
            if (update != null)
            {
                //_context.Entry(update).CurrentValues.SetValues(t);
                //_context.Entry(update).State = EntityState.Modified;

                _context.SaveChanges();
            }
            // }

            return t;
        }

        #endregion

        #region business layer

        public static List<UserTask> GetTasksWithoutDone(ApplicationDbContext _context)
        {
            List<UserTask> tasksAll = GetTasks(_context);
            List<UserTask> tasksWithoutDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.ToList<Done>();
            foreach (var t in tasksAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == DateTime.Now.Date);
                if (task_done == null)
                {
                    tasksWithoutDone.Add(t);
                }
            }

            return tasksWithoutDone;
        }

        public static List<UserTask> GetTasksOverdueNonPeriod(ApplicationDbContext _context)
        {
            List<UserTask> tasksAll = GetTasks(_context);
            List<UserTask> tasksOverdue = new List<UserTask>();
            List<Done> taskDones = _context.Dones.ToList<Done>();
            foreach (var t in tasksAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == DateTime.Now.Date);
                if (task_done == null)
                {
                    if (t.TaskType == 1)
                    {
                        if (t.DateStart != null)
                        {
                            DateTime date_end;
                            if (t.DateEnd == null && t.Duration != null)
                                date_end = t.DateStart.Value.AddMinutes((int)t.Duration);
                            else
                                date_end = (t.DateEnd == null) ? (DateTime)t.DateStart : (DateTime)t.DateEnd;

                            var dateShort = t.DateStart.Value.ToString();
                            if (t.DateStart.Value.TimeOfDay == TimeSpan.MinValue && dateShort != DateTime.Now.ToString())
                            {
                                tasksOverdue.Add(t);
                            }
                            else
                                if (t.DateStart.Value.TimeOfDay != TimeSpan.MinValue && date_end < DateTime.Now)
                            {
                                tasksOverdue.Add(t);
                            }
                        }
                    }
                }
            }

            return tasksOverdue;
        }

        public static List<UserTask> GetTasksWithoutDoneByDate(bool isCalendar, DateTime now, ApplicationDbContext _context)
        {
            now = now.AddDays(1).AddSeconds(-1);
            List<UserTask> todayAll = GetTasksByDate(isCalendar, now,_context);           
            todayAll = todayAll.FindAll(d => (d.DateActivate != null && d.DateActivate.Value.Date <= now.Date) || d.DateActivate == null || d.DateActivate == DateTime.MinValue);

            List<UserTask> todayWithoutDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone <= now).ToList<Done>();
            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == now.Date);
                if (task_done == null)
                {
                    todayWithoutDone.Add(t);
                }
            }

            return todayWithoutDone;
        }

        public static List<UserTask> GetTasksWithoutDoneWeekByDate(bool isCalendar, DateTime from, DateTime to, ApplicationDbContext _context)
        {
            List<UserTask> todayAll = GetTasksByDate(isCalendar, from, to,_context);
            List<UserTask> todayWithoutDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone >= from && x.DateDone <= to).ToList<Done>();
            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date >= from.Date && d.DateDone <= to.Date);
                if (task_done == null)
                {
                    todayWithoutDone.Add(t);
                }
            }

            return todayWithoutDone;
        }

        public static List<UserTask> GetTodayTasksWithoutDone(bool isCalendar, ApplicationDbContext _context)
        {
            List<UserTask> todayAll = GetTodayTasks(isCalendar, _context);
            List<UserTask> todayWithoutDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.ToList<Done>();
            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == DateTime.Now.Date);
                if (task_done == null)
                {
                    todayWithoutDone.Add(t);
                }
            }

            return todayWithoutDone;
        }

        public static List<UserTask> GetTasksDoneByDate(bool isCalendar, DateTime now, ApplicationDbContext _context)
        {
            now = now.AddDays(1).AddSeconds(-1);
            List<UserTask> todayAll = _context.UserTask.ToList<UserTask>();
            List<UserTask> todayDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone <= now).ToList<Done>();

            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == now.Date);
                if (task_done != null)
                {
                    todayDone.Add(t);
                }
            }

            return todayDone;
        }

        public static List<UserTask> GetTasksDoneWeekByDate(bool isCalendar, DateTime from, DateTime to, ApplicationDbContext _context)
        {
            List<UserTask> todayAll = GetTasksByDate(isCalendar, from, to,_context);
            List<UserTask> weekAll = GetTasksByDate(!isCalendar, from, to, _context);
            List<UserTask> todayDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone >= from && x.DateDone <= to).ToList<Done>();
            foreach (var t in weekAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date >= from.Date && d.DateDone <= to.Date);
                if (task_done != null)
                {
                    todayDone.Add(t);
                }
            }
            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date >= from.Date && d.DateDone <= to.Date);
                if (task_done != null)
                {
                    todayDone.Add(t);
                }
            }

            return todayDone;
        }

        public static List<UserTask> GetTodayTasksDone(bool isCalendar, ApplicationDbContext _context)
        {
            List<UserTask> todayAll = _context.UserTask.ToList<UserTask>();  // Task.GetTodayTasks(isCalendar);
            List<UserTask> todayDone = new List<UserTask>();
            List<Done> taskDones = _context.Dones.ToList<Done>();
            foreach (var t in todayAll)
            {
                Done task_done = taskDones.Find(d => d.TaskId == t.TaskId && d.UserId == t.UserId && d.DateDone.Date == DateTime.Now.Date);
                if (task_done != null)
                {
                    todayDone.Add(t);
                }
            }

            return todayDone;
        }

        public static List<UserTask> GetTasksByDate(bool isCalendar, DateTime now, ApplicationDbContext _context)
        {
            List<UserTask> tasks = _context.UserTask.ToList<UserTask>();
            tasks = tasks.FindAll(d => (d.DateActivate != null && d.DateActivate.Value.Date <= now.Date) || d.DateActivate == null || d.DateActivate == DateTime.MinValue);
            List<UserTask> tasks_period = new List<UserTask>();
            List<UserTask> tasks_non_period = new List<UserTask>();
            List<UserTask> tasks_result_calendar = new List<UserTask>();
            List<UserTask> tasks_result_non_calendar = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone <= now).ToList<Done>();

            foreach (UserTask t1 in tasks.Where(t => !String.IsNullOrWhiteSpace(t.Period)))
            {
                DateTime? dateCompare = t1.DateStart;
                if (t1.DateActivate != null) dateCompare = t1.DateActivate;

                if (dateCompare != null && ((DateTime)dateCompare).Date == now.Date || dateCompare == null)
                {
                    String[] dateVals = t1.Period.Split(' ');
                    Int32 days = (dateVals[2] != "*") ? Int32.Parse(dateVals[2]) : 0;
                    Int32 months = (dateVals[3] != "*") ? Int32.Parse(dateVals[3]) : 0;
                    Int32 weekDays = (dateVals[4] != "*") ? Int32.Parse(dateVals[4]) : -1;
                    Int32 mins = (dateVals[0] != "*") ? Int32.Parse(dateVals[0]) : -1;
                    Int32 hours = (dateVals[1] != "*") ? Int32.Parse(dateVals[1]) : -1;
                    int day = (int)now.DayOfWeek; // ((int)now.DayOfWeek == 0) ? 7 : (int)now.DayOfWeek;
                    if (weekDays >= 0)
                    {
                        if (weekDays == day)
                            tasks_period.Add(t1);
                    }
                    else
                    {
                        if (months > 0)
                        {
                            if (months == now.Month)
                                tasks_period.Add(t1);
                        }
                        else
                        {
                            if (days > 0)
                            {
                                if (days == now.Day)
                                    tasks_period.Add(t1);
                            }
                            else
                            {
                                if (hours >= 0 || mins >= 0)
                                    tasks_period.Add(t1);
                            }
                        }
                    }
                }
            }

            foreach (UserTask t1 in tasks.Where(t => String.IsNullOrWhiteSpace(t.Period)).ToList())
            {
                DateTime? dateCompare = t1.DateStart;
                if (t1.DateActivate != null) dateCompare = t1.DateActivate;

                if (dateCompare != null && ((DateTime)dateCompare).Date == now.Date || dateCompare == null)
                    tasks_non_period.Add(t1);
            }

            foreach (UserTask t in tasks_non_period)
            {
                if (t.DateStart == null)
                    tasks_result_non_calendar.Add(t);
                else
                {
                    if (t.DateStart.Value.TimeOfDay != TimeSpan.Zero)
                        tasks_result_calendar.Add(t);
                    else
                        tasks_result_non_calendar.Add(t);
                }
            }

            foreach (UserTask t in tasks_period)
            {
                String[] dateVals = t.Period.Split(' ');
                Int32 mins = (dateVals[0] != "*") ? Int32.Parse(dateVals[0]) : 0;
                Int32 hours = (dateVals[1] != "*") ? Int32.Parse(dateVals[1]) : 0;

                if (mins > 0 || hours > 0)
                {
                    t.DateStart = now.AddHours(hours).AddMinutes(mins);
                    if (t.Duration > 0)
                        t.DateEnd = t.DateStart.Value.AddMinutes((double)t.Duration);

                    tasks_result_calendar.Add(t);

                }
                else
                    tasks_result_non_calendar.Add(t);
            }

            return (isCalendar) ? tasks_result_calendar : tasks_result_non_calendar;//?
        }

        public static List<UserTask> GetTasksByDate(bool isCalendar, DateTime from, DateTime to, ApplicationDbContext _context)
        {
            List<UserTask> tasks = _context.UserTask.ToList<UserTask>();
            tasks = tasks.FindAll(d => (d.DateActivate != null && d.DateActivate.Value.Date <= to.Date) || d.DateActivate == null || d.DateActivate == DateTime.MinValue);
            List<UserTask> tasks_period = new List<UserTask>();
            List<UserTask> tasks_non_period = new List<UserTask>();
            List<UserTask> tasks_result_calendar = new List<UserTask>();
            List<UserTask> tasks_result_non_calendar = new List<UserTask>();
            List<Done> taskDones = _context.Dones.Where(x => x.DateDone >= from && x.DateDone <= to).ToList<Done>();

            foreach (UserTask t1 in tasks.Where(t => !String.IsNullOrWhiteSpace(t.Period)))
            {
                DateTime? dateCompare = t1.DateStart;
                if (t1.DateActivate != null) dateCompare = t1.DateActivate;

                if (dateCompare != null && ((DateTime)dateCompare).Date <= to.Date && ((DateTime)dateCompare).Date >= from.Date || dateCompare == null)
                {
                    String[] dateVals = t1.Period.Split(' ');
                    Int32 days = (dateVals[2] != "*") ? Int32.Parse(dateVals[2]) : 0;
                    Int32 months = (dateVals[3] != "*") ? Int32.Parse(dateVals[3]) : 0;
                    Int32 weekDays = (dateVals[4] != "*") ? Int32.Parse(dateVals[4]) : -1;
                    Int32 mins = (dateVals[0] != "*") ? Int32.Parse(dateVals[0]) : -1;
                    Int32 hours = (dateVals[1] != "*") ? Int32.Parse(dateVals[1]) : -1;

                    if (weekDays >= 0)
                    {
                        tasks_period.Add(t1);
                    }
                    else
                    {
                        if (months > 0)
                        {
                            if (months >= from.Month && months <= to.Month)
                                tasks_period.Add(t1);
                        }
                        else
                        {
                            if (days > 0)
                            {
                                if (days >= from.Day && days <= to.Day)
                                    tasks_period.Add(t1);
                            }
                            else
                            {
                                if (hours >= 0 || mins >= 0)
                                    tasks_period.Add(t1);
                            }
                        }
                    }
                }
            }

            foreach (UserTask t1 in tasks.Where(t => String.IsNullOrWhiteSpace(t.Period)).ToList())
            {
                DateTime? dateCompare = t1.DateStart;
                if (t1.DateActivate != null) dateCompare = t1.DateActivate;

                if (dateCompare != null && ((DateTime)dateCompare).Date <= to.Date || dateCompare == null)
                    tasks_non_period.Add(t1);
            }

            foreach (UserTask t in tasks_non_period)
            {
                if (t.DateStart == null)
                    tasks_result_non_calendar.Add(t);
                else
                {
                    if (t.DateStart.Value.TimeOfDay != TimeSpan.Zero)
                        tasks_result_calendar.Add(t);
                    else
                        tasks_result_non_calendar.Add(t);
                }
            }

            foreach (UserTask t in tasks_period)
            {
                String[] dateVals = t.Period.Split(' ');
                Int32 mins = (dateVals[0] != "*") ? Int32.Parse(dateVals[0]) : 0;
                Int32 hours = (dateVals[1] != "*") ? Int32.Parse(dateVals[1]) : 0;

                if (mins > 0 || hours > 0)
                {
                    tasks_result_calendar.Add(t);
                }
                else
                    tasks_result_non_calendar.Add(t);
            }

            return (isCalendar) ? tasks_result_calendar : tasks_result_non_calendar;//?
        }

        public static List<UserTask> GetTodayTasks(bool isCalendar, ApplicationDbContext _context)
        {
            return GetTasksByDate(isCalendar, DateTime.Now,_context);
        }

        #endregion

    }
}
