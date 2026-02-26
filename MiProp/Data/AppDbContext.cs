using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiProp.Models;

namespace MiProp.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Edificio> Edificios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Edificio>()
                .HasOne(e => e.Admin)
                .WithMany()
                .HasForeignKey(e => e.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Departamento>()
                .HasOne(d => d.Edificio)
                .WithMany(e => e.Departamentos)
                .HasForeignKey(d => d.EdificioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Departamento>()
                .HasOne(d => d.Inquilino)
                .WithMany()
                .HasForeignKey(d => d.InquilinoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Pago>()
                .HasOne(p => p.Inquilino)
                .WithMany()
                .HasForeignKey(p => p.InquilinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
