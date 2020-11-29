using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Tabula.Models
{
    public class Profile : IdentityUser
    {
        public byte[] Avatar { get; set; }
        public virtual ICollection<Board> Boards { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
