using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Api.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Products.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("ProductIdSequence")
            .StartsAt(100000)
            .IncrementsBy(1)
            .HasMin(100000)
            .HasMax(999999)
            .IsCyclic(false);

        modelBuilder.Entity<Product>(ConfigureProduct);
    }

    private static void ConfigureProduct(EntityTypeBuilder<Product> b)
    {
        b.HasKey(p => p.ProductId);

        b.Property(p => p.ProductId)
         .HasDefaultValueSql("NEXT VALUE FOR ProductIdSequence");
        
        b.Property(p => p.Price).HasPrecision(18, 2);
        b.HasIndex(p => p.Name);
    }
}