using Microsoft.EntityFrameworkCore;

namespace LazyMoon.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TTS> TTS { get; set; }
        public DbSet<ValorantRank> ValorantRanks { get; set; }
        public DbSet<Voice> Voices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
