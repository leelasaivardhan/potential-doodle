using CandidateRegistrationPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CandidateRegistrationPortal.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CandidateUser> CandidateUsers => Set<CandidateUser>();
    public DbSet<CandidateCourse> CandidateCourses => Set<CandidateCourse>();
    public DbSet<CandidateState> CandidateStates => Set<CandidateState>();
    public DbSet<CandidateDistrict> CandidateDistricts => Set<CandidateDistrict>();
    public DbSet<CandidateApplication> CandidateApplications => Set<CandidateApplication>();
    public DbSet<CandidateLoginAudit> CandidateLoginAudits => Set<CandidateLoginAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CandidateUser>().ToTable("CRP_CandidateUsers");
        modelBuilder.Entity<CandidateCourse>().ToTable("CRP_CandidateCourses");
        modelBuilder.Entity<CandidateState>().ToTable("CRP_CandidateStates");
        modelBuilder.Entity<CandidateDistrict>().ToTable("CRP_CandidateDistricts");
        modelBuilder.Entity<CandidateApplication>().ToTable("CRP_CandidateApplications");
        modelBuilder.Entity<CandidateLoginAudit>().ToTable("CRP_CandidateLoginAudits");

        modelBuilder.Entity<CandidateUser>().HasKey(x => x.UserId);
        modelBuilder.Entity<CandidateCourse>().HasKey(x => x.CourseId);
        modelBuilder.Entity<CandidateState>().HasKey(x => x.StateId);
        modelBuilder.Entity<CandidateDistrict>().HasKey(x => x.DistrictId);
        modelBuilder.Entity<CandidateApplication>().HasKey(x => x.ApplicationId);
        modelBuilder.Entity<CandidateLoginAudit>().HasKey(x => x.AuditId);

        modelBuilder.Entity<CandidateUser>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<CandidateUser>().HasIndex(x => x.MobileHash).IsUnique();

        modelBuilder.Entity<CandidateDistrict>()
            .HasOne(x => x.State)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CandidateApplication>()
            .HasOne(x => x.User)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CandidateApplication>()
            .HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CandidateApplication>()
            .HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CandidateApplication>()
            .HasOne(x => x.District)
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
