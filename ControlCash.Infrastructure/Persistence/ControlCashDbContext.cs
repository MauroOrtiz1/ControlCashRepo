using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ControlCash.Domain.Entities;

namespace ControlCash.Infrastructure.Persistence;

public partial class ControlCashDbContext : DbContext
{
    public ControlCashDbContext()
    {
    }

    public ControlCashDbContext(DbContextOptions<ControlCashDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Exportacion> Exportacions { get; set; }

    public virtual DbSet<Gasto> Gastos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        // => optionsBuilder.UseNpgsql("Host=localhost;Database=ControlCashDB;Username=postgres;Password=123456");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("categoria_pkey");

            entity.ToTable("categoria");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");

            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(100)
                .HasColumnName("nombre_categoria");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // ✅ Relación con Usuario
            entity.HasOne<Usuario>(c => c.Usuario)
                .WithMany(u => u.Categorias)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_usuario_categoria");
        });


        modelBuilder.Entity<Exportacion>(entity =>
        {
            entity.HasKey(e => e.IdExportacion).HasName("exportacion_pkey");

            entity.ToTable("exportacion");

            entity.Property(e => e.IdExportacion).HasColumnName("id_exportacion");

            // Cambia el tipo a 'timestamp with time zone' en lugar de 'timestamp without time zone'
            entity.Property(e => e.FechaExportado)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone") // Aquí es donde realizamos el cambio
                .HasColumnName("fecha_exportado");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.OrigenGrafico)
                .HasMaxLength(100)
                .HasColumnName("origen_grafico");
            entity.Property(e => e.TipoArchivo)
                .HasMaxLength(10)
                .HasColumnName("tipo_archivo");

            // Agregar los nuevos campos de mes y año
            entity.Property(e => e.MesExportado).HasColumnName("mes_exportado");
            entity.Property(e => e.AnioExportado).HasColumnName("anio_exportado");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Exportacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario_exportacion");
        });


        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.IdGasto).HasName("gasto_pkey");

            entity.ToTable("gasto");

            entity.Property(e => e.IdGasto).HasColumnName("id_gasto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Gastos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_categoria_gasto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Gastos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario_gasto");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Email, "usuario_email_key").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.AnunciosActivos)
                .HasDefaultValue(true)
                .HasColumnName("anuncios_activos");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EsPremium)
                .HasDefaultValue(false)
                .HasColumnName("es_premium");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasColumnName("rol")
                .HasDefaultValue("user");  
        });


        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
