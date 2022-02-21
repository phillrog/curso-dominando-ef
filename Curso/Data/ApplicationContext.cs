using System;
using System.IO;
using System.Reflection;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("Log_sistema.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Conversor> Conversores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Server=localhost,1435;Database=DevIO2; User=sa; Password=yourStrong#@Teste;Trusted_Connection=False;Persist Security Info=False; pooling=false;MultipleActiveResultSets=True";
            optionsBuilder
                .UseSqlServer(strConnection,
                p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    .MaxBatchSize(100)
                    .CommandTimeout(5)
                    .EnableRetryOnFailure(4, TimeSpan.FromSeconds(10), null)
                )
                .EnableSensitiveDataLogging()                
                // .UseLazyLoadingProxies()
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
            // modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

            modelBuilder.Entity<Departamento>().Property(p => p.Descricao)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");

            modelBuilder.HasSequence("MinhaSequencia","sequencias").StartsAt(1)
            .IncrementsBy(2)
            .HasMin(1)
            .HasMax(10)
            .IsCyclic();

            // modelBuilder.Entity<Departamento>().Property(p => p.Id)
            // .HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");

            // modelBuilder
            //     .Entity<Departamento>()
            //     .HasIndex(p => new { p.Descricao, p.Ativo})
            //     .HasDatabaseName("idx_meu_indice_composto")
            //     .HasFilter("DESCRICAO IS NOT NULL")
            //     .HasFillFactor(80)
            //     .IsUnique();

            // modelBuilder.Entity<Departamento>()
            //     .HasData(new [] {
            //         new Estado() {  Id = 1, Nome = "São Paulo"}
            //     });
            // modelBuilder.HasDefaultSchema("cadastros");
            //  modelBuilder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");


            // var conversao = new ValueConverter<Versao, string>(p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p));
            // var conversao1 = new EnumToStringConverter<Versao>();

            //  modelBuilder.Entity<Conversor>()
            //     .Property(p => p.Versao)
            //     .HasConversion(conversao1);
            //.HasConversion(conversao);
            //.HasConversion(p=>p.ToString(), p=> (Versao)Enum.Parse(typeof(Versao), p));
            //.HasConversion<string>();

            // modelBuilder.Entity<Conversor>()
            //     .Property(p => p.Status)
            //     .HasConversion(new Curso.Conversores.ConversorCustomizado());

            // modelBuilder.Entity<Departamento>().Property<DateTime>("UltimaAtualizacao");    
            // modelBuilder.Entity<Cliente>(p => {
            //     p.OwnsOne(x=> x.Endereco, end => {
            //         end.Property(p => p.Bairro).HasColumnName("Bairro");
            //         end.ToTable("Endereco");
            //     });
            // });

            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }

        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }
    }
}