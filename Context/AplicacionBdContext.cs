using LABOFMUTANTAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LABOFMUTANTAPI.Context;
public sealed class AplicacionBdContext(DbContextOptions<AplicacionBdContext> options) : DbContext(options)
{
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
    public DbSet<Perfil> Perfiles => Set<Perfil>();
    public DbSet<Preferencia> Preferencias => Set<Preferencia>();
    public DbSet<Progreso> Progresos => Set<Progreso>();
    public DbSet<NotaDiario> NotasDiario => Set<NotaDiario>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Comentario>(entity => {
            entity.ToTable("Comentarios"); entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AutorId, x.IdCliente }).IsUnique();
            entity.Property(x => x.AutorId).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.Nombre).HasMaxLength(40);
            entity.Property(x => x.Texto).HasMaxLength(600);
        });
        model.Entity<Dispositivo>(entity => {
            entity.ToTable("Dispositivos"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64).IsUnicode(false);
        });
        model.Entity<Perfil>(entity => {
            entity.ToTable("Perfiles"); entity.HasKey(x => x.DispositivoId);
            entity.Property(x => x.DispositivoId).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.Nombre).HasMaxLength(40);
            entity.HasOne<Dispositivo>().WithOne().HasForeignKey<Perfil>(x => x.DispositivoId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<Preferencia>(entity => {
            entity.ToTable("Preferencias"); entity.HasKey(x => x.DispositivoId);
            entity.Property(x => x.DispositivoId).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.PestanaGuia).HasMaxLength(10).IsUnicode(false);
            entity.HasOne<Dispositivo>().WithOne().HasForeignKey<Preferencia>(x => x.DispositivoId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<Progreso>(entity => {
            entity.ToTable("Progresos"); entity.HasKey(x => x.DispositivoId);
            entity.Property(x => x.DispositivoId).HasMaxLength(64).IsUnicode(false);
            entity.HasOne<Dispositivo>().WithOne().HasForeignKey<Progreso>(x => x.DispositivoId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<NotaDiario>(entity => {
            entity.ToTable("NotasDiario"); entity.HasKey(x => new { x.DispositivoId, x.IdLocal });
            entity.Property(x => x.DispositivoId).HasMaxLength(64).IsUnicode(false);
            entity.Property(x => x.IdLocal).ValueGeneratedNever();
            entity.Property(x => x.Texto).HasMaxLength(600);
            entity.HasOne<Dispositivo>().WithMany().HasForeignKey(x => x.DispositivoId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
