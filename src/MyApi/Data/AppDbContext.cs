using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace MyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserEntity> Users => Set<UserEntity>();
    }

    public class UserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public int Age { get; set; }
    }

}
