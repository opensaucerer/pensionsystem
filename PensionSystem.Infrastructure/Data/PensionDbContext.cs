using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Data;

public class PensionDbContext : DbContext
{
    public PensionDbContext(DbContextOptions<PensionDbContext> options) : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<Benefit> Benefits => Set<Benefit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            
            entity.HasMany(e => e.Contributions)
                  .WithOne(c => c.Member)
                  .HasForeignKey(c => c.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Benefits)
                  .WithOne(b => b.Member)
                  .HasForeignKey(b => b.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(50);
        });

        builder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(100);
        });

        builder.Entity<Benefit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        });
    }
}
