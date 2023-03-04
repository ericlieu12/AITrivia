using AITrivia.DBModels;
using Microsoft.EntityFrameworkCore;
namespace AITrivia.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Lobby> Lobbys { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 

        }

    }
}
