using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tabula.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nickname")]
        public string Name { get; set; }

        [Display(Name = "Profile photo")]
        public IFormFile AvatarFile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Wrong password!")]
        [DataType(DataType.Password)]
        [Display(Name = "Verify password")]
        public string PasswordConfirm { get; set; }
    }
}
