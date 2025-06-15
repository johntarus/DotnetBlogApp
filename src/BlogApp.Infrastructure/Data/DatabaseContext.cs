using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){}
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>().HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Like>().HasOne(l => l.User)
            .WithMany(u=>u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Comment>().HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>().HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Post>().HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Post>().HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity(j => j.ToTable("PostTags"));
        
        base.OnModelCreating(modelBuilder);
    }
}