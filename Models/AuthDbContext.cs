using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationsSso.Models;

public partial class AuthDbContext : DbContext
{
    public AuthDbContext()
    {
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Authorization> Authorizations { get; set; }

    public virtual DbSet<AuthorizationAppClient> AuthorizationAppClients { get; set; }

    public virtual DbSet<TestTable> TestTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // => optionsBuilder.UseSqlServer("Server=Progr;Database=WebPortalTest;MultipleActiveResultSets=true;Encrypt=False;Trusted_Connection=true");
        => optionsBuilder.UseSqlServer("Server=192.168.60.33;Database=WebPortal;User Id=sa;Password=123456Aa;MultipleActiveResultSets=true;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Authorization>(entity =>
        {
            entity.ToTable("Authorization");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Iin)
                .HasMaxLength(12)
                .HasColumnName("IIN");
        });

        modelBuilder.Entity<AuthorizationAppClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AuthorizationAppClients");

            entity.ToTable("AuthorizationAppClient");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.SystemName).HasMaxLength(50);
        });

        modelBuilder.Entity<TestTable>(entity =>
        {
            entity.ToTable("TestTable");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
