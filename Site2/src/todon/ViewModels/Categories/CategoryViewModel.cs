using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todon.Models;

namespace todon.ViewModels.Categories
{
    public class CategoryViewModel
    {
        public Int32 CategoryID { get; set; }
        public Int32? ParentID { get; set; }
        [Display(Name = "Parent name")]
        public String ParentName { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя категории")]
        public String Name { get; set; }
        public String Picture { get; set; }
        public Int32? UserId { get; set; }
        public String Owner { get; set; }
        public bool IsEditable { get; set; }
        public String PictureFullPath { get { return "~/Content/Uploaded/" + this.Picture; } }

        public List<SelectListItem> PublicCategories { get; set; }
        public List<SelectListItem> UserCategories { get; set; }
        public List<SelectListItem> UserCategoriesRoot { get; set; }

        public CategoryViewModel(ApplicationDbContext _context, Int32 userId)
        {
            PublicCategories = new List<SelectListItem>();
            UserCategories = new List<SelectListItem>();
            UserCategoriesRoot = new List<SelectListItem>();
            List<Category> cats = Category.GetCategories(_context);
            PublicCategories.Add(new SelectListItem() { Value = "0", Text = "Public" });
            foreach (var c in cats)
            {
                    PublicCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            } 
                
            UserCategories.Add(new SelectListItem() { Value = "0", Text = "User" });
            foreach (var c in cats.Where(x => x.UserId==userId))
            {
                UserCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }
            UserCategoriesRoot.Add(new SelectListItem() { Value = "0", Text = "User" });
            foreach (var c in cats.Where(x => x.UserId == userId && x.ParentID == 0 || x.UserId == userId && x.ParentID == null))
            {
                UserCategoriesRoot.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }

        }

        public CategoryViewModel(ApplicationDbContext _context, Category category)
        {
            PublicCategories = new List<SelectListItem>();
            UserCategories = new List<SelectListItem>();
            UserCategoriesRoot = new List<SelectListItem>();
            List<Category> cats = Category.GetCategories(_context);
            PublicCategories.Add(new SelectListItem() { Value = "0", Text = "Public" });
            foreach (var c in cats)
            {
                PublicCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }

            UserCategories.Add(new SelectListItem() { Value = "0", Text = "User" });
            foreach (var c in cats.Where(x => x.UserId == category.UserId))
            {
                UserCategories.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }
            UserCategoriesRoot.Add(new SelectListItem() { Value = "0", Text = "User" });
            foreach (var c in cats.Where(x => x.UserId == category.UserId && x.ParentID==0|| x.UserId == category.UserId && x.ParentID ==null))
            {
                UserCategoriesRoot.Add(new SelectListItem() { Value = c.CategoryID.ToString(), Text = c.Name });
            }

            Name = category.Name;
            CategoryID = category.CategoryID;
            Picture = category.Picture;
            UserId = category.UserId;
            ParentID = category.ParentID;
        }

        public CategoryViewModel()
        {
            PublicCategories = new List<SelectListItem>();
            PublicCategories.Add(new SelectListItem() { Value = "0", Text = "Public" });
        }

    }
}


// public Int32? Parent_id { get; set; }
//protected List<Category> Children;
// public List<Task> Tasks { get; set; }