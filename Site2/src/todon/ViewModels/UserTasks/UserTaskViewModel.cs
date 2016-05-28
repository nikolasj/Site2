using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todon.Models;

namespace todon.ViewModels.UserTasks
{
    public class UserTaskViewModel
    {
        public Int32 TaskId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, укажите название!")]
        public String Name { get; set; }
        public String Description { get; set; }
      //  public String Picture { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите категорию!")]
        public Int32 CategoryId { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Пожалуйста, выберите категорию!")]
        public String CategoryName { get; set; }
        [Display(Name = "Category Picture")]
        public String CategoryPicture { get; set; }
        public Int32 ParentId { get; set; }
        [Display(Name = "Parent Name")]
        public String ParentName { get; set; }
        public Int32 UserId { get; set; }
        [Display(Name = "User Name")]
        public String UserName { get; set; }
        public Int32 Priority { get; set; }
        [Display(Name = "Priority")]
        public String PriorityName { get; set; }
        public DateTime? DateCreate { get; set; }
        [Display(Name = "Task type")]
        public Byte TaskType { get; set; }
        [Display(Name = "Type")]
        public String TaskTypeName { get; set; }
        public String Period { get; set; }
        [Display(Name = "Date Start")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "Date Start")]
        public String DateStartStr { get; set; }
        [Display(Name = "Duration")]
        public Int32 Duration { get; set; }
        [Display(Name = "Duration")]
        public String DurationStr { get; set; }
        [Display(Name = "Date End")]
        public DateTime? DateEnd { get; set; }
        [Display(Name = "Date End")]
        public String DateEndStr { get; set; }
        public String Place { get; set; }
        public Int32 TimeHour { get; set; }
        public DateTime Date { get; set; }
       // public String PictureFullPath { get { return "~/Content/Uploaded/" + this.Picture; } }

        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Parents { get; set; }
        public List<SelectListItem> Priorities { get; set; }
        public List<SelectListItem> TaskTypes { get; set; }

        public UserTaskViewModel() { }

        public UserTaskViewModel(ApplicationDbContext _context, UserTask userTask)
        {
            this.TaskId = userTask.TaskId;
            this.Name = userTask.Name;
            this.ParentId = (userTask.ParentId == null) ? 0 : (int)userTask.ParentId;
            UserTask parent = null;
            if (_context.UserTask.Any(x => x.TaskId == this.ParentId))
                parent = _context.UserTask.First(x => x.TaskId == this.ParentId);
            this.ParentName = (parent == null) ? "No" : parent.Name;
            this.CategoryId = userTask.CategoryId;
            Category cat;
            if (this.CategoryId != 0)
            {
                cat = Category.GetCategories(_context).First(x => x.CategoryID == this.CategoryId);
            }
            else
            {
                cat = null;
            }
            this.CategoryName = (cat == null) ? "No" : cat.Name;
            this.DateCreate = userTask.Date_create;
            this.DateStart = userTask.DateStart;
            this.DateEnd = userTask.DateEnd;
            this.Description = userTask.Description;
            this.DurationStr = Helper.ConvertMinsToDuration(userTask.Duration);
            this.TaskTypeName = (userTask.TaskType == 1) ? "once" : "periodic";
            this.Period = userTask.Period;
            this.Place = userTask.Place;
            this.TaskType = userTask.TaskType;
           // this.Picture = userTask.Picture;
            this.Priority = (userTask.Priority == null) ? 0 : (int)userTask.Priority;
            if (this.CategoryId != 0)
            {
                this.CategoryPicture = Category.GetCategoryById(userTask.CategoryId, _context).Picture;
            }
            else
            {
                this.CategoryPicture = "";
            }
            switch (userTask.Priority)
            {
                case 1:
                    this.PriorityName = "lowest";
                    break;
                case 2:
                    this.PriorityName = "low";
                    break;
                case 3:
                    this.PriorityName = "medium";
                    break;
                case 4:
                    this.PriorityName = "heightened";
                    break;
                case 5:
                    this.PriorityName = "highest";
                    break;

            }
            this.UserId = (userTask.UserId == null) ? 0 : (int)userTask.UserId;

            Categories = new List<SelectListItem>();
            Parents = new List<SelectListItem>();
            Priorities = new List<SelectListItem>();
            TaskTypes = new List<SelectListItem>();

            List<Category> cats = Category.GetCategories(_context).Where(c => c.UserId == null || c.UserId == this.UserId).ToList();
            Categories.Add(new SelectListItem() { Value = "0", Text = "No" });
            foreach (var c in cats)
            {
                Categories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }
            List<UserTask> tasks = UserTask.GetTasks(_context).Where(c => c.UserId == this.UserId).ToList();
            Parents.Add(new SelectListItem() { Value = "0", Text = "No" });
            foreach (var t in tasks)
            {
                Parents.Add(new SelectListItem() { Value = t.TaskId.ToString(), Text = t.Name });
            }

            Priorities.Add(new SelectListItem() { Value = 1.ToString(), Text = "lowest" });
            Priorities.Add(new SelectListItem() { Value = 2.ToString(), Text = "low" });
            Priorities.Add(new SelectListItem() { Value = 3.ToString(), Text = "medium" });
            Priorities.Add(new SelectListItem() { Value = 4.ToString(), Text = "heightened" });
            Priorities.Add(new SelectListItem() { Value = 5.ToString(), Text = "highest" });

            TaskTypes.Add(new SelectListItem() { Value = "1", Text = "once" });
            TaskTypes.Add(new SelectListItem() { Value = "2", Text = "periodic" });
        }

        public UserTaskViewModel(ApplicationDbContext _context, Int32 userId)
        {
            Categories = new List<SelectListItem>();
            Parents = new List<SelectListItem>();
            Priorities = new List<SelectListItem>();
            TaskTypes = new List<SelectListItem>();

            List<Category> cats = Category.GetCategories(_context).Where(c=>c.UserId==null||c.UserId==userId).ToList();
            Categories.Add(new SelectListItem() { Value = "0", Text = "No" });
            foreach (var c in cats)
            {
                Categories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }
            List<UserTask> tasks = UserTask.GetTasks(_context).Where(c => c.UserId == userId).ToList();
            Parents.Add(new SelectListItem() { Value = "0", Text = "No" });
            foreach (var t in tasks)
            {
                Parents.Add(new SelectListItem() { Value = t.TaskId.ToString(), Text = t.Name });
            }

            Priorities.Add(new SelectListItem() { Value = 1.ToString(), Text = "lowest" });
            Priorities.Add(new SelectListItem() { Value = 2.ToString(), Text = "low" });
            Priorities.Add(new SelectListItem() { Value = 3.ToString(), Text = "medium" });
            Priorities.Add(new SelectListItem() { Value = 4.ToString(), Text = "heightened" });
            Priorities.Add(new SelectListItem() { Value = 5.ToString(), Text = "highest" });

            TaskTypes.Add(new SelectListItem() { Value = "1", Text = "once" });
            TaskTypes.Add(new SelectListItem() { Value = "2", Text = "periodic" });

            this.UserId = userId;
        }
    }
}


//public List<SelectListItem> PublicCategories { get; set; }
//public List<SelectListItem> UserCategories { get; set; }

//public CategoryViewModel(ApplicationDbContext _context, Int32 userId)
//{
//    PublicCategories = new List<SelectListItem>();
//    UserCategories = new List<SelectListItem>();
//    List<Category> cats = Category.GetCategories(_context);
//    PublicCategories.Add(new SelectListItem() { Value = "0", Text = "Public" });
//    foreach (var c in cats)
//    {
//        PublicCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
//    }

//    UserCategories.Add(new SelectListItem() { Value = "0", Text = "User" });
//    foreach (var c in cats.Where(x => x.UserId == userId))
//    {
//        UserCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
//    }
//}

//public CategoryViewModel()
//{
//    PublicCategories = new List<SelectListItem>();
//    PublicCategories.Add(new SelectListItem() { Value = "0", Text = "Public" });
//}