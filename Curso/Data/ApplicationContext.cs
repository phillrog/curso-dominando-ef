using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Server=localhost,1435;Database=DevIO2; User=sa; Password=yourStrong#@Teste;Trusted_Connection=False;Persist Security Info=False; pooling=false";
            optionsBuilder
                .UseSqlServer(strConnection)
                .EnableSensitiveDataLogging();
                //.LogTo(Console.WriteLine, LogLevel.Information);
        }        
    }
}