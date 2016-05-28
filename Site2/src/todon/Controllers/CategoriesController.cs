using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using todon.Models;
using todon.ViewModels.Categories;
using System.Collections.Generic;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using System;
using Microsoft.AspNet.Hosting;

namespace todon.Controllers
{

    [Authorize]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _environment;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
            _userManager = userManager;    
        }

        #region CategoryView
        // GET: Categories
        public Task<IActionResult> Index()
        {
            return CategoriesView();
        }

        public async Task<IActionResult> CategoriesView()
        {          
           UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            bool isAdmin = false;
            var userAsp = await _userManager.FindByNameAsync(user.Mail);
            isAdmin = await _userManager.IsInRoleAsync(userAsp, "Admin");

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            if (user != null)
            {
                CategoriesListViewModel clvm = new CategoriesListViewModel();
                clvm.Categories = new List<CategoryViewModel>();


                foreach (Category category in Category.GetCategories(_context))
                {
                    if (category.UserId == null)
                    {
                        category.UserId = null; 
                        CategoryViewModel uvm = new CategoryViewModel(_context, category);
                        uvm.IsEditable = isAdmin;
                        clvm.Categories.Add(uvm);
                    }

                    if (category.UserId == user.UserId)
                    {
                        CategoryViewModel uvm = new CategoryViewModel(_context,category);
                        uvm.IsEditable = true;
                        clvm.IsEditable = uvm.IsEditable;
                        clvm.Categories.Add(uvm);
                    }
                    
                }
                return View("CategoriesView", clvm);
            }
            return RedirectToAction("Index");
        }

        // GET: Categories/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Category category = _context.Category.FirstOrDefault(m => m.CategoryID == id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        #endregion

        #region Create User

        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);

            return View("Create", cvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public IActionResult Create(Category category)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (ModelState.IsValid)
            {              
                category.UserId= user.UserId;
                //category.Picture = "http://worldtemplates.net/uploads/posts/2012-07/1341132303_1325433392_partner.jpg";
                _context.Category.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }          
            CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);
            return View(cvm);
        }

        [HttpPost]
        public ActionResult Upload(IFormFile file)//, int Id, string Title)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            var fileName = Helper.UploadFile(file, _environment.WebRootPath);
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);
                cvm.Picture = fileName;
                return View("Create", cvm);
            }
            return Json(new { Status = "Error" });
        }

        #endregion

        #region Create Admin
        [Authorize(Roles = "Admin")]
        public IActionResult CreatePublic()
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);

            return View("CreatePublic", cvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreatePublic(Category category)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            if (ModelState.IsValid)
            {
                category.Picture = "http://prestashop.modulez.ru/132-thickbox/block-of-categories-on-the-homepage.jpg";
                category.ParentID = null;         
                _context.Category.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);
            return View(cvm);
        }

        [HttpPost]
        public ActionResult UploadAdmin(IFormFile file)//, int Id, string Title)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            var fileName = Helper.UploadFile(file, _environment.WebRootPath);
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                CategoryViewModel cvm = new CategoryViewModel(_context, user.UserId);
                cvm.Picture = fileName;
                return View("CreatePublic", cvm);
            }
            return Json(new { Status = "Error" });
        }

        #endregion

        #region Edit

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            bool isAdmin = false;
            // UserManager<ApplicationUser> _userManager;
            var userAsp = await _userManager.FindByNameAsync(user.Mail);
            isAdmin = await _userManager.IsInRoleAsync(userAsp, "Admin");
            
            if (id == null)
            {
                return HttpNotFound();
            }

            Category category = _context.Category.FirstOrDefault(m => m.CategoryID == id);
            if (category == null||category.UserId!=user.UserId&&!isAdmin)
            {
                return HttpNotFound();
            }
            if (category == null || category.UserId != null && isAdmin)
            {
                return HttpNotFound();
            }
            // if (!isAdmin && category.UserId == null) return Details(category.CategoryID);            
            CategoryViewModel cvm = new CategoryViewModel(_context, category);

            cvm.ParentID = (category.ParentID != null) ? (int)category.ParentID : 0;
            if (cvm.ParentID != 0)
                cvm.ParentName = _context.Category.FirstOrDefault(m => m.CategoryID == cvm.ParentID).ToString();
            else
                cvm.ParentName = string.Empty;
            if(category.UserId!=null)
                cvm.IsEditable = true;

            return View("Edit",cvm);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            bool isAdmin = false;
            var userAsp = await _userManager.FindByNameAsync(user.Mail);
            isAdmin = await _userManager.IsInRoleAsync(userAsp, "Admin");

            if (ModelState.IsValid)
            {
                if (isAdmin)
                {
                    category.UserId = null;
                    _context.Update(category);
                    _context.SaveChanges();
                }
                else
                {
                    category.UserId = user.UserId;
                    _context.Update(category);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            CategoryViewModel cvm = new CategoryViewModel(_context, category);
            return View(cvm);
        }

        [HttpPost]
        public ActionResult UploadEdit(IFormFile file)//, int Id, string Title)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            var fileName = Helper.UploadFile(file, _environment.WebRootPath);
            if (!String.IsNullOrWhiteSpace(fileName))
            {

                CategoryViewModel cvm = new CategoryViewModel(_context,user.UserId);
                cvm.CategoryID = Int32.Parse(Request.Form["CategoryID"]);
                cvm.Name= Request.Form["Name"];
                cvm.ParentName = "No";
                cvm.ParentID= Int32.Parse(Request.Form["ParentID"]);
                cvm.Picture = fileName;
                cvm.UserId = user.UserId;
                cvm.IsEditable = true;
                return View("Edit",cvm);
            }
            return Json(new { Status = "Error" });
        }

        #endregion

        #region Delete

        // GET: Categories/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            bool isAdmin = false;
            // UserManager<ApplicationUser> _userManager;
            var userAsp = await _userManager.FindByNameAsync(user.Mail);
            isAdmin = await _userManager.IsInRoleAsync(userAsp, "Admin");

            if (id == null)
            {
                return HttpNotFound();
            }

            Category category = _context.Category.FirstOrDefault(m => m.CategoryID == id);
            if (category == null || category.UserId != user.UserId && !isAdmin)
            {
                return HttpNotFound();
            }
            if (category == null || category.UserId != null && isAdmin)
            {
                return HttpNotFound();
            }
            if (!isAdmin && category.UserId == null) return Details(category.CategoryID);
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            UserProfile user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
            bool isAdmin = false;
            // UserManager<ApplicationUser> _userManager;
            var userAsp = await _userManager.FindByNameAsync(user.Mail);
            isAdmin = await _userManager.IsInRoleAsync(userAsp, "Admin");
            
            Category category = _context.Category.FirstOrDefault(m => m.CategoryID == id);
            CategoryViewModel cvm = new CategoryViewModel(_context, category);
            //if (System.IO.File.Exists(cvm.PictureFullPath))
            //    System.IO.File.Delete(cvm.PictureFullPath);
           // if (!isAdmin && category.UserId == null) return Details(category.CategoryID);
            _context.Category.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

    }
}
