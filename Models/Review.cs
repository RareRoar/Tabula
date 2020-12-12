using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tabula.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public bool Liked { get; set; }
        public string Comment { get; set; }
        public int PinId { get; set; }
        public virtual Pin Pin { get; set; }
        public string ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
