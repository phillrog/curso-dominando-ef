using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDeleted();
            GapEnsureCreated();
        }

        static void EnsureCreatedAndDeleted() {
            using var db = new Curso.Data.ApplicationContext();
            //db.Database.EnsureCreated();
            db.Database.EnsureDeleted();
        }
        /// Todas vez que EnsureCreated é executado é feito uma validação se já foram criados
        /// as tabelas como contexto cidade ele checa isso ele não vai criar a tabela cidade porque já identificou a criação
        /// para contornar isso é feito a criação "manualmente" com databaseCreator
        static void GapEnsureCreated() {
            using var db1 = new Curso.Data.ApplicationContext();
            using var db2 = new Curso.Data.ApplicationContextCidade();

            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
    }
}
