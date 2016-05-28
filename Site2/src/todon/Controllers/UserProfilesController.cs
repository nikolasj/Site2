using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using todon.Models;
using Microsoft.AspNet.Authorization;
using todon.ViewModels.UserProfiles;
using Microsoft.AspNet.Http;
using System.IO;
using Microsoft.AspNet.Hosting;
using System;

namespace todon.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public UserProfilesController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;    
        }

        // GET: UserProfiles
        public IActionResult Index()
        {
            if (ModelState.IsValid)
            {
                UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
                if (user != null)
                {
                    UserProfileViewModel uvm = new UserProfileViewModel(user);
                    return View("Index",uvm);
                }
            }
            return RedirectToAction("Index");
        }

        // GET: UserProfiles/Details/5
        public IActionResult Details(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }

            UserProfile userProfile = _context.UserProfile.FirstOrDefault(m => m.UserId == id);
            if (userProfile == null|| userProfile.UserId != user.UserId)
            {
                return HttpNotFound();
            }
            UserProfileViewModel uvm = new UserProfileViewModel(userProfile);
            return View(uvm);
        }      

        // GET: UserProfiles/Edit/5
        public IActionResult Edit(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }

            UserProfile userProfile = _context.UserProfile.FirstOrDefault(m => m.UserId == id);
            if (userProfile == null || userProfile.UserId != user.UserId)
            {
                return HttpNotFound();
            }
            UserProfileViewModel upvm = new UserProfileViewModel(userProfile);
           
            return View(upvm);
        }
               
        [HttpPost]
        public ActionResult Upload(IFormFile file)//, int Id, string Title)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            var fileName = Helper.UploadFile(file, _environment.WebRootPath);
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                user.Picture = fileName;
                return View("Edit", new UserProfileViewModel(user));
            }
            return Json(new { Status = "Error" });
        }
               
        // POST: UserProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {               
                _context.Update(userProfile);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userProfile);
        }
    
    }
}
