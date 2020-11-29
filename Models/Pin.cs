using System.Collections.Generic;

namespace Tabula.Models
{
    public class Pin
    {
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public string BoardId { get; set; }
        public virtual Board Board { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
