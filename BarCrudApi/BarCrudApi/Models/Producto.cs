using BarCrudApi.ViewModels;
using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class Producto
    {
        //constructores
        public Producto()
        {
            PedidoDetalles = new HashSet<PedidoDetalle>();
        }
        public Producto(ProductoViewModel productoVM)
        {
            Nombre = productoVM.Nombre;
            Precio = productoVM.Precio;
            Imagen = productoVM.Imagen;
            Descripcion = productoVM.Descripcion;
            CategoriaId = productoVM.CategoriaId;
            Stock = productoVM.Stock;
        }
        public Producto(ProductoAdminViewModel productoVM)
        {
            Nombre = productoVM.Nombre;
            Descripcion = productoVM.Descripcion;
            Precio = productoVM.Precio;
            Imagen = productoVM.Imagen;
            FechaBaja = productoVM.FechaBaja;
            CategoriaId = productoVM.CategoriaId;
            BarId = productoVM.BarId;
            Stock = productoVM.Stock;
        }
        public Producto (ProductoManagerViewModel productoVM) 
        {
            Nombre = productoVM.Nombre;
            Descripcion = productoVM.Descripcion;
            Precio = productoVM.Precio;
            Imagen = productoVM.Imagen;
            CategoriaId = productoVM.CategoriaId;
            Stock = productoVM.Stock;
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        public int Stock { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int CategoriaId { get; set; }
        public int BarId { get; set; }

        public virtual Bar Bar { get; set; } = null!;
        public virtual Categoria Categoria { get; set; } = null!;
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; }
    }
}


