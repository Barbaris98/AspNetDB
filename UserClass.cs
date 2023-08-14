using Microsoft.EntityFrameworkCore;

namespace AspNetDB
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // имя пользователя
        public int Age { get; set; } // возраст пользователя
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated(); //создаём БД при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Tom", Age = 37 },
                new User { Id = 2, Name = "Bob", Age = 41 },
                new User { Id = 3, Name = "Sam", Age = 24 }
                );
        }
    }


}
