using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace todon.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя!")]
        public string UserName { get; set; }
        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Mail { get; set; }
        public string Picture { get; set; }

        public static List<UserProfile> GetUsers(ApplicationDbContext _context)
        {
            return _context.UserProfile.ToList<UserProfile>();//?
        }
        public static UserProfile AddUser(UserProfile u, ApplicationDbContext _context)
        {
            _context.UserProfile.Add(u);//?
            _context.SaveChanges();
            return u;
        }
    }
}
