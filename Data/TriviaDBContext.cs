using Microsoft.EntityFrameworkCore;
using TriviaGameApp.Models;

public class TriviaDBContext : DbContext
{
    public TriviaDBContext(DbContextOptions<TriviaDBContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    // Add other DbSets for your other entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)

    
    {
        modelBuilder.Entity<User>().ToTable("User");
        // Add other entities here
    }
}
// Path: Data/TriviaDBContext.cs
