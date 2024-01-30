using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.DBContext
{
    public class KalumDBContext : DbContext
    {
        // propiedades de Entities
        public DbSet<CarreraTecnica> CarrerasTecnicas { get; set; }
        public DbSet<Jornada> Jornadas { get; set; }
        public DbSet<ExamenAdmision> ExamenesAdmision { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }

        public DbSet<Aspirante> Aspirantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarreraTecnica>().ToTable("CarreraTecnica").HasKey(ct => new { ct.CarreraId });
            modelBuilder.Entity<Jornada>().ToTable("Jornada").HasKey(j => new {j.JornadaId});
            modelBuilder.Entity<ExamenAdmision>().ToTable("ExamenAdmision").HasKey(e => new {e.ExamenId});
            modelBuilder.Entity<Alumno>().ToTable("Alumno").HasKey(a => new {a.Carne});
            modelBuilder.Entity<Aspirante>().ToTable("Aspirante").HasKey(s => new {s.NoExpediente});
            // carrera Tecnica
            modelBuilder.Entity<Aspirante>().HasOne<CarreraTecnica>(a => a.CarreraTecnica).WithMany(ct => ct.Aspirantes).HasForeignKey(asp => asp.CarreraId);
            // Jornada
            modelBuilder.Entity<Aspirante>().HasOne<Jornada>(a => a.Jornada).WithMany(j => j.Aspirantes).HasForeignKey(asp => asp.JornadaId);
            //Examen
            modelBuilder.Entity<Aspirante>().HasOne<ExamenAdmision>(a => a.ExamenAdmision).WithMany(ea => ea.Aspirantes).HasForeignKey(asp => asp.ExamenId);
        }

        public KalumDBContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}