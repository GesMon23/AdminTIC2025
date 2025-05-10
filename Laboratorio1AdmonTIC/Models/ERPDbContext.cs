using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Laboratorio1AdmonTIC.Models
{
    public class ERPDbContext : IdentityDbContext<IdentityUser>
    {
        public ERPDbContext(DbContextOptions<ERPDbContext> options)
            : base(options)
        {


        }
        //public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Municipio> Municipios { get; set; }

		public DbSet<Departamento> Departamento { get; set; }

		public DbSet<Empleados> Empleados { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Relación uno a uno entre Empleado y ApplicationUser
            //modelBuilder.Entity<Empleados>()
            //    .HasOne(e => e.User) // Un Empleado tiene un User
            //    .WithOne(u => u.Empleados) // Un User tiene un Empleado
            //    .HasForeignKey<Empleados>(e => e.UserId) // Especificamos que UserId es la FK
            //    .OnDelete(DeleteBehavior.Restrict); // Opcional: controlar la eliminación (en caso de necesidad)

            // Relación entre DetallesCompras y Compras
        //    modelBuilder.Entity<DetallesCompras>()
        //        .HasOne(d => d.Compras)
        //        .WithMany()  // No hay navegación inversa en este caso
        //        .HasForeignKey(d => d.CompraId)
        //        .OnDelete(DeleteBehavior.Restrict);  // Desactivar eliminación en cascada

        //    // Relación entre DetallesCompras y Productos
        //    modelBuilder.Entity<DetallesCompras>()
        //        .HasOne(d => d.Productos)
        //        .WithMany()  // No hay navegación inversa en este caso
        //        .HasForeignKey(d => d.ProductoId)
        //        .OnDelete(DeleteBehavior.Restrict);  // Desactivar eliminación en cascada
        }

        public DbSet<MetodosPago> MetodosPago { get; set; }

		public DbSet<Clientes> Clientes { get; set; }

		public DbSet<CategoriasProducto> CategoriasProducto { get; set; }

		public DbSet<Proveedores> Proveedores { get; set; }

		public DbSet<Productos> Productos { get; set; }

		public DbSet<TiposMovimiento> TiposMovimiento { get; set; }

        public DbSet<Compras> Compras { get; set; }

        public DbSet<DetallesCompras> DetallesCompras { get;set; }

        public DbSet<Inventario> Inventario { get; set; }

        public DbSet<DetallesVenta> DetallesVenta { get; set; }

        public DbSet<Ventas> Ventas { get; set; }
	}
}
