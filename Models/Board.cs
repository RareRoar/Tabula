using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tabula.Models
{
    public class Board
    {
       
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<Pin> Pins { get; set; }
    }
}
