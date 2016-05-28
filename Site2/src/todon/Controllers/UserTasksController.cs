using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using todon.Models;
using todon.ViewModels.UserTasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace todon.Controllers
{
    [Authorize]
    public class UserTasksController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public UserTasksController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: UserTasks

        public IActionResult Index()
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            List<UserTaskViewModel> utvml = new List<UserTaskViewModel>();
            List<UserTask> utl = _context.UserTask.Where(x => x.UserId == user.UserId).ToList();          
            foreach (UserTask t in utl)
            {
                UserTaskViewModel utvm = new UserTaskViewModel(_context, t);                
                utvml.Add(utvm);
            }
            return View(utvml);
        }

        // GET: UserTasks/Details/5
        public IActionResult Details(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }

            UserTask userTask = _context.UserTask.FirstOrDefault(m => m.TaskId == id);
            if (userTask == null|| userTask.UserId!=user.UserId)
            {
                return HttpNotFound();
            }
            UserTaskViewModel utvm = new UserTaskViewModel(_context,userTask);
            
            return View(utvm);
        }

        #region Create

        // GET: UserTasks/Create
        public IActionResult Create()
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            UserTaskViewModel cvm = new UserTaskViewModel(_context, user.UserId);
            cvm.Period = "0 0 * * 1";
            return View("Create",cvm);
        }

        // POST: UserTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserTask userTask)
        {
           
           ModelValidationState entry = ModelState.GetFieldValidationState("Duration");
            if (entry == ModelValidationState.Invalid)
            {
                ModelStateEntry durationEntry = ModelState["Duration"];
                userTask.Duration = Helper.ConvertDurationToInt32(durationEntry.RawValue.ToString());
                ModelState["Duration"].RawValue = userTask.Duration;
                ModelState["Duration"].ValidationState = ModelValidationState.Valid;
            }
             UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (ModelState.IsValid)
            {
                userTask.Date_create = DateTime.Now;
                userTask.UserId = user.UserId;

                _context.UserTask.Add(userTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            UserTaskViewModel utvm = new UserTaskViewModel(_context, userTask);
         
            return View(utvm);
        }

        public IActionResult CreateTaskByDateTime(string taskDate)
        {
            UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (u != null)
            {
                UserTaskListViewModel tlvm = new UserTaskListViewModel();
                tlvm.Tasks = new List<UserTaskViewModel>();
                // int date_start_hour = 0;
                DateTime dateValue = DateTime.Parse(taskDate);


                List<Category> cats = Category.GetCategories(_context);
                UserTaskViewModel cvm = new UserTaskViewModel(_context, u.UserId);
                cvm.DateStart = dateValue;
                cvm.DateStartStr = dateValue.ToString("s");
                return View("Create", cvm);
            }
            return View("Index");
        }       

        #endregion

        #region Edit

        // GET: UserTasks/Edit/5
        public IActionResult Edit(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }

            UserTask userTask = _context.UserTask.FirstOrDefault(m => m.TaskId == id);
            if (userTask == null || userTask.UserId != user.UserId)
            {
                return RedirectToAction("Index");
            }

            UserTaskViewModel utvm = new UserTaskViewModel(_context, userTask);
            utvm.DateStart = userTask.DateStart;
            if (utvm.DateStart != null)
            {
                DateTime dateValueStart = DateTime.Parse(userTask.DateStart.ToString());
                utvm.DateStartStr = dateValueStart.ToString("s");
            }

            utvm.DateEnd = userTask.DateEnd;
            if (utvm.DateEnd != null)
            {
                DateTime dateValueEnd = DateTime.Parse(userTask.DateEnd.ToString());
                utvm.DateEndStr = dateValueEnd.ToString("s");
            }
            utvm.Period = "0 0 * * 1";
            return View("Edit",utvm);
        }

        // POST: UserTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
                userTask.Duration = Helper.ConvertDurationToInt32(Request.Form["Duration"]);
                userTask.UserId = user.UserId;
                
               // userTask.DateActivate = DateTime.Now;
                _context.Update(userTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            UserTaskViewModel utvm = new UserTaskViewModel(_context, userTask);

            return View(utvm);
        }
        

        #endregion

        #region Delete

        // GET: UserTasks/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }

            UserTask userTask = _context.UserTask.FirstOrDefault(m => m.TaskId == id);
            if (userTask == null || userTask.UserId != user.UserId)
            {
                return HttpNotFound();
            }

            return View(userTask);
        }

        // POST: UserTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            UserTask userTask = _context.UserTask.FirstOrDefault(m => m.TaskId == id);
            _context.UserTask.Remove(userTask);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion        

    }
}



//[HttpPost]
//public ActionResult Upload(IFormFile file)//, int Id, string Title)
//{
//    UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//    var fileName = Helper.UploadFile(file, _environment.WebRootPath);
//    if (!String.IsNullOrWhiteSpace(fileName))
//    {
//        UserTaskViewModel utvm = new UserTaskViewModel(_context, user.UserId);
//        utvm.Picture = fileName;
//        return View("Create", utvm);
//    }
//    return Json(new { Status = "Error" });
//}
//[HttpPost]
//public ActionResult UploadEdit(IFormFile file, UserTask userTask)//, int Id, string Title)
//{
//    UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
//    var fileName = Helper.UploadFile(file, _environment.WebRootPath);
//    if (!String.IsNullOrWhiteSpace(fileName))
//    {
//        userTask.Picture = fileName;
//        userTask.Name = Request.Form["Name"];

//        userTask.ParentId = Int32.Parse(Request.Form["ParentID"]);
//        userTask.CategoryId = Int32.Parse(Request.Form["CategoryId"]);
//        userTask.Picture = fileName;
//        userTask.UserId = user.UserId;
//        userTask.Place = Request.Form["Place"];

//        userTask.Description = Request.Form["Description"];
//        userTask.DateStart = ((Request.Form["DateStart"]).ToString() == "") ? (DateTime?)null : (DateTime?)DateTime.Parse(Request.Form["DateStart"]);
//        userTask.DateEnd = ((Request.Form["DateEnd"]).ToString() == "") ? (DateTime?)null : (DateTime?)DateTime.Parse(Request.Form["DateEnd"]);
//        UserTaskViewModel utvm = new UserTaskViewModel(_context, userTask);
//        //utvm.Picture = fileName;
//        //utvm.Name = Request.Form["Name"];
//        //utvm.ParentName = Request.Form["ParentName"];
//        //utvm.ParentId = Int32.Parse(Request.Form["ParentID"]);
//        //utvm.CategoryId = Int32.Parse(Request.Form["CategoryId"]);
//        //utvm.Picture = fileName;
//        //utvm.UserId = user.UserId;
//        //utvm.Place = Request.Form["Place"];
//        //utvm.CategoryName = Request.Form["CategoryName"];
//        //utvm.Description = Request.Form["Description"];
//        //utvm.DateStart = ((Request.Form["DateStart"]).ToString() == "") ? (DateTime?)null : (DateTime?)DateTime.Parse(Request.Form["DateStart"]);
//        //utvm.DateEnd = ((Request.Form["DateEnd"]).ToString() == "") ? (DateTime?)null : (DateTime?)DateTime.Parse(Request.Form["DateEnd"]);

//        return View("Edit", utvm);
//    }
//    return Json(new { Status = "Error" });
//}


//userTask.CategoryId = Int32.Parse(Request.Form["CategoryName"]);
//userTask.DateEnd = DateTime.Parse(Request.Form["DateEnd"]);
//userTask.DateStart = DateTime.Parse(Request.Form["DateStart"]);
//userTask.Description = Request.Form["Description"];
//userTask.Duration= Int32.Parse(Request.Form["Duration"]);
//userTask.Name = Request.Form["Name"];
//userTask.ParentId= Int32.Parse(Request.Form["ParentName"]);
//userTask.Period= Request.Form["PeriodName"];
//userTask.Picture= Request.Form["Picture"];
//userTask.Place= Request.Form["Place"];
//userTask.Priority= Int32.Parse(Request.Form["PriorityName"]);
//userTask.TaskType= Byte.Parse(Request.Form["TaskTypeName"]);


//utvm.TaskId = t.TaskId;
//utvm.Name = t.Name;
//utvm.ParentId = (t.ParentId == null) ? 0 : (int)t.ParentId;
//UserTask parent = null;
//if (_context.UserTask.Any(x => x.TaskId == utvm.ParentId))
//    parent = _context.UserTask.First(x => x.TaskId == utvm.ParentId);
//utvm.ParentName = (parent == null) ? "No" : parent.Name;
//utvm.CategoryId = t.CategoryId;
//Category cat = Category.GetCategories(_context).First(x => x.CategoryID == utvm.CategoryId);
//utvm.CategoryName = (cat == null) ? "No" : cat.Name;
//utvm.DateCreate = t.Date_create;
//utvm.DateStart = t.DateStart;
//utvm.DateEnd = t.DateEnd;
//utvm.Description = t.Description;
//utvm.DurationStr = t.Duration.ToString();
//utvm.TaskTypeName = (t.TaskType == 1) ? "once" : "periodic";
//utvm.Picture = t.Picture;
//utvm.Period = t.Period;
//switch (t.Priority)
//{
//    case 1:
//        utvm.PriorityName = "lowest";
//        break;
//    case 2:
//        utvm.PriorityName = "low";
//        break;
//    case 3:
//        utvm.PriorityName = "medium";
//        break;
//    case 4:
//        utvm.PriorityName = "heightened";
//        break;
//    case 5:
//        utvm.PriorityName = "highest";
//        break;

//}