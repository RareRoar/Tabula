using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tabula.Interfaces;

namespace Tabula.Models
{
    public class AppDbContext : IdentityDbContext<Profile>, IApplicationDbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Pin> Pins { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(p => p.Pin)
                .WithMany(t => t.Reviews)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Pin>()
                 .HasOne(p => p.Board)
                 .WithMany(t => t.Pins)
                 .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Board>()
                 .HasOne(p => p.Profile)
                 .WithMany(t => t.Boards)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Review>()
                 .HasOne(p => p.Profile)
                 .WithMany(t => t.Reviews)
                 .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<Pin>()
                   .HasMany(p => p.Reviews)
                   .WithOne(t => t.Pin)
                   .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Board>()
                .HasMany(p => p.Pins)
                .WithOne(t => t.Board)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Reviews)
                .WithOne(t => t.Profile)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Boards)
                .WithOne(t => t.Profile)
                .OnDelete(DeleteBehavior.Cascade);*/
        }
    }
}
