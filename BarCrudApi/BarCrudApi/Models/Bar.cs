using BarCrudApi.ViewModels;
using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class Bar
    {
        //constructores
        public Bar()
        {
            Productos = new HashSet<Producto>();
        }
        public Bar(BarViewModel barVM)
        {
            Id = barVM.Id;
            Nombre = barVM.Nombre;
            Direccion = barVM.Direccion;
        }
        public Bar(BarAdminViewModel barVM)
        {
            Id = barVM.Id;
            Nombre = barVM.Nombre;
            Direccion = barVM.Direccion;
            FechaBaja = barVM.FechaBaja;
            ManagerDni = barVM.ManagerDni;
            Imagen = barVM.Imagen;
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? ManagerDni { get; set; }
        public string? Imagen { get; set; }

        public virtual Persona? ManagerDniNavigation { get; set; }
        public virtual ICollection<Producto> Productos { get; set; }
    }
}

