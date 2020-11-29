using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tabula.Models;

namespace Tabula.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Board> Boards { get; set; }
        DbSet<Pin> Pins { get; set; }
        DbSet<Review> Reviews { get; set; }
        Task<int> SaveChangesAsync();
    }
}
