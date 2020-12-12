using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tabula.Models;

namespace Tabula.ViewModels
{
    public class ReviewViewModel
    {

        [Required]
        [Display(Name = "Is that way you like it?")]
        public bool Liked { get; set; }

        [Required]
        [Display(Name = " ")]
        public string Comment { get; set; }
    
        [Required]
        public int PinId { get; set; }
    }
}
