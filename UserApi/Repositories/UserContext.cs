using Microsoft.EntityFrameworkCore;
using UserApi.Entities;

namespace UserApi.Repositories {
    public class UserContext : DbContext {
        public UserContext(DbContextOptions<UserContext> options) : base(options){}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("UserService.User");
        }
    }
}