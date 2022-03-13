using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src.Domain;
using src.Extensions;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<UsuarioFuncao> UsuarioFuncoes { get; set; }
        
        public DbSet<DepartamentoRelatorio> DepartamentoRelatorio { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=tips; Integrated Security=true")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UsuarioFuncao>().HasNoKey();

            modelBuilder.Entity<DepartamentoRelatorio>(e=>
            {
                e.HasNoKey();

                e.ToView("vw_departamento_relatorio");

                e.Property(p=>p.Departamento).HasColumnName("Descricao");
            });

            var properties = modelBuilder.Model.GetEntityTypes()
                .SelectMany(p=>p.GetProperties())
                .Where(p=> p.ClrType == typeof(string)
                        && p.GetColumnType() == null);

            foreach(var property in properties)
            {
                property.SetIsUnicode(false);
            }     

            modelBuilder.ToSnakeCaseNames();    
        }
    }
}