using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Unique constraints 
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.RoleName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Role 1-to-many User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        // User 1-to-1 Profile 
        modelBuilder.Entity<Profile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // User (Organiser) 1-to-many Event 
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organiser)
            .WithMany(u => u.OrganisedEvents)
            .HasForeignKey(e => e.OrganiserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Event 1-to-many Category 
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.EventID)
            .OnDelete(DeleteBehavior.Cascade);

        // Event 1-to-many Enrolment
        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.Event)
            .WithMany(e => e.Enrolments)
            .HasForeignKey(en => en.EventID)
            .OnDelete(DeleteBehavior.Restrict);

        // User (Participant) 1-to-many Enrolment
        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.Participant)
            .WithMany(u => u.Enrolments)
            .HasForeignKey(en => en.ParticipantID)
            .OnDelete(DeleteBehavior.Restrict);

        // Category 1-to-many Enrolment
        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.Category)
            .WithMany(c => c.Enrolments)
            .HasForeignKey(en => en.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrolment 1-to-1 Result 
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(en => en.Result)
            .HasForeignKey<Result>(r => r.EnrolmentID)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate enrolment 
        modelBuilder.Entity<Enrolment>()
            .HasIndex(en => new { en.ParticipantID, en.EventID })
            .IsUnique();

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleID = 1, RoleName = "Organiser" },
            new Role { RoleID = 2, RoleName = "Participant" }
        );
    }
}