using Microsoft.EntityFrameworkCore;
using UserApi.Entities;

namespace UserApi.Repositories {
    public class UserContext : DbContext {
        public UserContext(DbContextOptions<UserContext> options) : base(options){}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.Entity<User>().ToTable("user");

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}