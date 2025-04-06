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
        public DbSet<Municipio> Municipios { get; set; }

		public DbSet<Departamento> Departamento { get; set; }

		public DbSet<Empleados> Empleados { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación uno a uno entre Empleado y ApplicationUser
            modelBuilder.Entity<Empleados>()
                .HasOne(e => e.User) // Un Empleado tiene un User
                .WithOne(u => u.Empleados) // Un User tiene un Empleado
                .HasForeignKey<Empleados>(e => e.UserId) // Especificamos que UserId es la FK
                .OnDelete(DeleteBehavior.Restrict); // Opcional: controlar la eliminación (en caso de necesidad)
        }

        public DbSet<MetodosPago> MetodosPago { get; set; }

		public DbSet<Clientes> Clientes { get; set; }

		public DbSet<CategoriasProducto> CategoriasProducto { get; set; }

		public DbSet<Proveedores> Proveedores { get; set; }

		public DbSet<Productos> Productos { get; set; }

		public DbSet<TiposMovimiento> TiposMovimiento { get; set; }
	}
}
