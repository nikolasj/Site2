using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todon.Models;

namespace todon.ViewModels.UserProfiles
{
    public class UserProfileViewModel
    {
        public Int32 UserId { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите имя!")]
        [Display(Name ="User name")]
        public String UserName { get; set; }
        public String Mail { get; set; }
        public String Picture { get; set; }
        public Int32 CountCategory { get; set; }
        public Int32 CountTasks { get; set; }
        public Int32 CountDoneTasks { get; set; }
        public String PictureFullPath { get { return "~/Content/Uploaded/" + this.Picture; } }

        public UserProfileViewModel(UserProfile user)
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.Mail = user.Mail;
            this.Picture = (user.Picture == null) ? "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a6/Anonymous_emblem.svg/200px-Anonymous_emblem.svg.png" : user.Picture;
        }

        public UserProfileViewModel() { }
    }
}
