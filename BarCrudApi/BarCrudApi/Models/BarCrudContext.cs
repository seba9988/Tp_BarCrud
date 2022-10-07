using System;
using System.Collections.Generic;
using BarCrudApi.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BarCrudApi.Models
{
    public partial class BarCrudContext : BarIdentityContext
    {
        public BarCrudContext(DbContextOptions<BarCrudContext> options)
    : base(options)
        {
        }

        public virtual DbSet<Bar> Bares { get; set; } = null!;
        public virtual DbSet<Categoria> Categorias { get; set; } = null!;
        public virtual DbSet<Pedido> Pedidos { get; set; } = null!;
        public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bar>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaBaja).HasColumnType("datetime");

                entity.Property(e => e.ManagerDni)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Imagen)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.ManagerDniNavigation)
                    .WithMany(p => p.Bares)
                    .HasForeignKey(d => d.ManagerDni)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Bares_Manager");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaBaja).HasColumnType("datetime");

                entity.Property(e => e.Imagen)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.ClienteDni)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCompra).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.ClienteDniNavigation)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.ClienteDni)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Pedidos_Clientes");
            });

            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.HasKey(e => new { e.PedidoId, e.ProductoId });

                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.Pedido)
                    .WithMany(p => p.PedidoDetalles)
                    .HasForeignKey(d => d.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PedidoDetalles_Pedidos");

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.PedidoDetalles)
                    .HasForeignKey(d => d.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PedidoDetalles_Productos");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.Dni)
                    .HasName("PK_Cliente");

                entity.Property(e => e.Dni)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Apellido)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaBaja).HasColumnType("datetime");

                entity.Property(e => e.IdUsuario).HasMaxLength(450);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.FechaBaja).HasColumnType("datetime");

                entity.Property(e => e.Imagen)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Precio).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.Bar)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.BarId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Productos_Bares");

                entity.HasOne(d => d.Categoria)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Productos_Categorias");
            });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
