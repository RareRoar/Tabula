using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabula.Models
{
    public class Review
    {
        public string Id { get; set; }
        public bool Liked { get; set; }
        public string Comment { get; set; }
        public string PinId { get; set; }
        public virtual Pin Pin { get; set; }
        public string ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
