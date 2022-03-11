using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src.Domain;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Pessoa> Pessoas {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //const string strConnection = "Data source=(localdb)\\mssqllocaldb; Initial Catalog=DevIO-03;Integrated Security=true;";
            const string strConnectionPG = "Host=localhost;Database=DEVIO04;Username=postgres;Password=123";

            optionsBuilder
                //.UseSqlServer(strConnection)
                //.UseNpgsql(strConnectionPG)
                //.UseSqlite("Data source=devio04.db")
                //.UseInMemoryDatabase(databaseName: "teste-devio")

                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "dev-io04")

                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>(conf=> 
            {
                conf.HasKey(p=>p.Id);
                
                conf.ToContainer("Pessoas");
                
                //conf.Property(p=>p.Nome).HasMaxLength(60).IsUnicode(false);
            });
        }
    }
}