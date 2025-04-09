namespace LoginApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }


        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Check if there's any admin user in the database, if not, seed one
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "admin", // You can change this to your admin username
                    Password = "admin", // Set a default password or hash it
                    IsLocked = false,
                    IsLoggedIn = false,
                    IsAdmin = true // This makes the user an admin
                }
            );
        }*/
    }
}