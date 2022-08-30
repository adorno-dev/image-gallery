using ImageGallery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Web.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) {}

        public DbSet<Gallery> Galleries => Set<Gallery>();

        public DbSet<Image> Images => Set<Image>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Image>()
              .ToTable("Image");

            mb.Entity<Gallery>()
              .ToTable("Gallery");
        }
    }
}