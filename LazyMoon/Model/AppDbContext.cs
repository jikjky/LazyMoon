using Microsoft.EntityFrameworkCore;

namespace LazyMoon.Model
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TTS> TTS { get; set; }
        public DbSet<ValorantRank> ValorantRanks { get; set; }
        public DbSet<Voice> Voices { get; set; }
        public DbSet<ConnectionHistory> ConnectionHistorys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
