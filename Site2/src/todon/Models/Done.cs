using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todon.Models
{
    public class Done
    {
        [Key]
        public Int32 DoneId { get; set; }
        public Int32 TaskId { get; set; }
        public DateTime DateDone { get; set; }
        public Int32 UserId { get; set; }

        public static Done AddDone(Done d, ApplicationDbContext _context)
        {
            _context.Dones.Add(d);//?
            _context.SaveChanges();
            return d;
        }


        public static bool DeleteDone(Done d, ApplicationDbContext _context)
        {
            var update = _context.Dones.FirstOrDefault(m => m.DoneId == d.DoneId);
            //var update = _context.Dones.Remove(d);//?
            if (update != null)
            {
                _context.Dones.Remove(update);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public static List<Done> GetTodayDone(ApplicationDbContext _context)
        {
            List<Done> dones = _context.Dones.ToList<Done>();
            return dones.Where(t => t.DateDone != null && ((DateTime)t.DateDone).Date == DateTime.Now.Date).ToList();
        }

        public static List<Done> GetDoneByPeriod(DateTime date_start, DateTime date_finish, ApplicationDbContext _context)
        {
            List<Done> dones = _context.Dones.ToList<Done>();
            List<Done> dones_task = dones.Where(d => d.DateDone >= date_start && d.DateDone <= date_finish).ToList();
            return dones_task;
        }

        public static List<Done> GetDoneByDate(DateTime date, ApplicationDbContext _context)
        {
            DateTime date_start = date.Date;
            DateTime date_done = date_start.AddDays(1);
            List<Done> dones = _context.Dones.ToList<Done>();
            List<Done> dones_task = dones.Where(d => d.DateDone >= date_start && d.DateDone < date_done).ToList();
            return dones_task;
        }
    }
}


//[Key]
//public Int32 TaskId { get; set; }
//public String Name { get; set; }
//public String Description { get; set; }
//public String Picture { get; set; }
//// [ForeignKey("Category")]
//public Int32 CategoryId { get; set; }
//public Int32? ParentId { get; set; }
//public Int32? UserId { get; set; }
//public Int32? Priority { get; set; }
//public DateTime Date_create { get; set; }
//public Byte TaskType { get; set; }
//public String Period { get; set; }
//public DateTime? DateStart { get; set; }
//public Int32? Duration { get; set; }
//public DateTime? DateEnd { get; set; }
//public String Place { get; set; }
//public DateTime? DateActivate { get; set; }