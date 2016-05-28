using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Server;
using todon.Models;
using Microsoft.AspNet.Authorization;
using todon.ViewModels.UserProfiles;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;

namespace todon.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IHostingEnvironment _environment;
        private ApplicationDbContext _context;
        // UserProfile user; 
        public HomeController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
            // user = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
        }

        public IActionResult Index()
        {
            UserProfileViewModel uvm = new UserProfileViewModel();
            try
            {
                UserProfile u = UserProfile.GetUsers(_context).Find(x => x.Mail.ToLower() == User.Identity.Name.ToLower());
                if (u != null)
                {

                    uvm.UserId = u.UserId;
                    uvm.UserName = u.UserName;
                    uvm.Mail = u.Mail;
                    uvm.Picture = (u.Picture == null) ? "" : u.Picture;
                }
            }
            catch (Exception ex)
            {

            }
                return View(uvm);
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "Your application description page.";
          //  DbAccessLayer db;
          //  var cats = DbAccessLayer.Categories;
            return View();

        }

        //[HttpPost]
        //public ActionResult Upload(IFormFile file)//, int Id, string Title)
        //{

        //    if (file.Length > 0)
        //    {
        //        var targetDirectory = Path.Combine(_environment.WebRootPath, string.Format("Content\\Uploaded\\"));
        //        var fileName = GetFileName(file);
        //        var savePath = Path.Combine(targetDirectory, fileName);

        //        file.SaveAs(savePath);
        //        return Json(new { Status = "Ok" });
        //    }
        //    return Json(new { Status = "Error" });
        //}
        //private static string GetFileName(IFormFile file) => file.ContentDisposition.Split(';')
        //                                                        .Select(x => x.Trim())
        //                                                        .Where(x => x.StartsWith("filename="))
        //                                                        .Select(x => x.Substring(9).Trim('"'))
        //                                                        .First();


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}


//[HttpPost]
//public async Task<IActionResult> Upload()
//{
//   // Stream bodyStream = Context.Request.Body;

//    //using (FileStream fileStream = File.Create(string.Format(@"C:\{0}", fileName)))
//    //{

//    //    await bodyStream.CopyToAsync(fileStream);

//    //}

//    return new HttpStatusCodeResult(200);
//}


//public async Task<IActionResult> Setup()
//{
//    var user = await userManager.FindByIdAsync(User.GetUserId());

//    var adminRole = await roleManager.FindByNameAsync("Admin");
//    if (adminRole == null)
//    {
//        adminRole = new IdentityRole("Admin");
//        await roleManager.CreateAsync(adminRole);

//        await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, "projects.view"));
//        await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, "projects.create"));
//        await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, "projects.update"));
//    }

//    if (!await userManager.IsInRoleAsync(user, adminRole.Name))
//    {
//        await userManager.AddToRoleAsync(user, adminRole.Name);
//    }

//    var accountManagerRole = await roleManager.FindByNameAsync("Account Manager");

//    if (accountManagerRole == null)
//    {
//        accountManagerRole = new IdentityRole("Account Manager");
//        await roleManager.CreateAsync(accountManagerRole);

//        await roleManager.AddClaimAsync(accountManagerRole, new Claim(CustomClaimTypes.Permission, "account.manage"));
//    }

//    if (!await userManager.IsInRoleAsync(user, accountManagerRole.Name))
//    {
//        await userManager.AddToRoleAsync(user, accountManagerRole.Name);
//    }

//    return Ok();
//}