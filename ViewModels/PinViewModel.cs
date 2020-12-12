using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Tabula.ViewModels
{
    public class PinViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Pin image")]
        public IFormFile Image { get; set; }

        public IEnumerable<string> BoardTitles { get; set; }

        [Display(Name = "Board")]
        public string BoardTitle { get; set; }

    }
}
