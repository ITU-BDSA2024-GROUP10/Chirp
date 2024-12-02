using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext(DbContextOptions<ChirpDBContext> options) : IdentityDbContext<Author>(options)
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //define cheep entity constraints
        modelBuilder.Entity<Cheep>(entity =>
        {
            //define auto increment key
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            //define required fields
            entity.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(160);
            entity.Property(c => c.TimeStamp)
                .IsRequired();

            //define foreign relation
            entity.HasOne(c => c.Author)
                .WithMany(a => a.Cheeps)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        //define author entity constraints
        modelBuilder.Entity<Author>(entity =>
        {
            //define required + unique name
            entity.Property(a => a.UserName)
                .IsRequired();
            entity.Property(a => a.Email)
                .IsRequired();
            entity.HasIndex(a => a.UserName)
                .IsUnique();
            entity.HasIndex(a => a.Email);

            entity.HasMany(a => a.Following)
                .WithMany(a => a.Followers);
        });

        //define comment entity constraints
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey((c => c.Id));
            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(160);
            entity.Property(c => c.TimeStamp)
                .IsRequired();
            entity.HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Cheep)
                .WithMany(c => c.Comments)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id).ValueGeneratedOnAdd();

            entity.HasOne(l => l.Author)
                .WithMany(a => a.Likes)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Cheep)
                .WithMany(c => c.Likes)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}