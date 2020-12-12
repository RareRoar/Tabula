using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tabula.Models
{
    public class Pin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public int BoardId { get; set; }
        public virtual Board Board { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
