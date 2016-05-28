using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todon.Models
{
    public class Category
    {
        [Key]
        public Int32 CategoryID { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя категории!")]
        //[StringLength(50, ErrorMessage = "Value should be less than 50 symbols.")]
        public String Name { get; set; }
        public String Picture { get; set; }
        public Int32? UserId { get; set; }
        public Int32? ParentID { get; set; }


        public static List<Category> GetCategories(ApplicationDbContext _context)
        {
            List<Category> cats = _context.Category.ToList<Category>();

            return cats;
        }

        public static Category GetCategoryById(int category_id, ApplicationDbContext _context)
        {
            Category cats = Category.GetCategories(_context).First(x => x.CategoryID == category_id);
            return cats;
        }
    }
}
