using System;
using System.IO;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("Log_sistema.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Server=localhost,1435;Database=DevIO2; User=sa; Password=yourStrong#@Teste;Trusted_Connection=False;Persist Security Info=False; pooling=false;MultipleActiveResultSets=True";
            optionsBuilder
                .UseSqlServer(strConnection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies()
                .LogTo(Console.WriteLine, LogLevel.Information);
                // .LogTo(Console.WriteLine, new [] {CoreEventId.ContextInitialized, RelationalEventId.CommandExecuted},
                // LogLevel.Information,
                // DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine
                // );
                // .LogTo(_writer.WriteLine, LogLevel.Information);
                // .EnableDetailedErrors();
        }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Departamento>().HasQueryFilter(p=>!p.Excluido);
        }

        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }
    }
}