using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
    {

        // Database Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User Table (Entity)
            modelBuilder.Entity<User>(entity =>
            {
                // Primary Key (Id)
                entity.HasKey(user => user.Id);

                // Unique Constaint on Email + Index (Frequent Referencing in Register/Login)
                entity.HasIndex(user => user.Email).IsUnique();

                // Constraints
                entity.Property(user => user.Name).HasMaxLength(255);
                entity.Property(user => user.Email).HasMaxLength(255).IsRequired();
                entity.Property(user => user.Password).HasMaxLength(255).IsRequired();
            });

            // Configure Todos Table
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.HasKey(e => e.Id);


                // Constraints
                entity.Property(todo => todo.Title).HasMaxLength(255).IsRequired();
                // No Constraints for Description Column (Yet)

                // Store TodoStatus enum as a string (rather than implicitly as an int)
                entity.Property(todo => todo.Status).HasConversion<string>();


                // Relationship: Todo -> User (Many-to-one)
                entity.HasOne(todo => todo.User)      // Each Todo has ONE User
                        .WithMany(user => user.Todos)   // Each User has MANY Todos
                        .HasForeignKey(todo => todo.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            });
        }

    }
}