using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

namespace MyFirstApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Register models and generate db tables
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //* Soft Delete
        // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        // {
        //     if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
        //     {
        //         var method = typeof(AppDbContext)
        //         .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
        //         ?.MakeGenericMethod(entityType.ClrType);

        //         method?.Invoke(null, new object[] { modelBuilder });
        //     }
        // }

        //* User entity config
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasIndex(u => u.IsActive);
            entity.HasIndex(u => u.IsSupperAdmin);
            entity.HasIndex(u => u.Role);

            entity.HasMany(u => u.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        //* Category entity config
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique(true);

            entity.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(c => c.ParentId).IsRequired(false);

            entity.HasMany(c => c.Posts)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        //* Post entity config
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique(true);
            entity.HasIndex(p => p.Title);

            entity.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => new { t.Name, t.IsActive });
            entity.HasIndex(t => t.Slug).IsUnique();
            entity.Property(t => t.IsActive).HasDefaultValue(true);
        });
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
    where TEntity : class, ISoftDeletable
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}