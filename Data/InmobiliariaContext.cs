using Inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Data
{
    public class InmobiliariaContext : DbContext
    {
        public InmobiliariaContext(DbContextOptions<InmobiliariaContext> options) : base(options) { }

        public DbSet<Propietario> Propietarios => Set<Propietario>();
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Propietario>().ToTable("propietarios");
            
            // Si existe alguna clase Persona y NO querés tabla:
            // modelBuilder.Ignore<Persona>();
            base.OnModelCreating(modelBuilder);
        }
    }
}