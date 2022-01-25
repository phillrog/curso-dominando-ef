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
            //EnsureCreatedAndDeleted(); // 1
            //GapEnsureCreated(); // 2
            //HealthCheckBancoDeDadosAgora(); // 3

            // new Curso.Data.ApplicationContext().Departamentos.AsNoTracking().Any(); // 4
            // GerenciarEstadoDaConexao(false); // 4
            // GerenciarEstadoDaConexao(true); // 4


        }

        static void SqlInjection(){
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Curso.Domain.Departamento {
                    Descricao = "Departamento 01"
                },
                new Curso.Domain.Departamento {
                    Descricao = "Departamento 02"
                }
            );

            db.SaveChanges();
            
            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao} ");
            }

            /// Correto
            var descricao = "Departamento 01";
            db.Database.ExecuteSqlRaw("update departamentos set descricao ='Departamento alterado' where descricao={0}", descricao);

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao} ");
            }

            /// Errado
            descricao = "Teste ' OR 1='1'";
            db.Database.ExecuteSqlRaw($"update departamentos set descricao ='AtaqueSQLInjection' where descricao={descricao}");

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao} ");
            }
        }
        static void ExecuteSQL(){
            using var db = NovaConexao();
            // Forma 1 - criando comando
            using (var cmd = db.Database.GetDbConnection().CreateCommand()){
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // Forma 2 - sql explicito com parametros
            var descricao = "Teste";
            db.Database.ExecuteSqlRaw("update departamentos set descricap={0} where Id = 1", descricao);

            // Forma 3 - parametros em interpolação
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricap={descricao} where Id = 1");
        }
        static Curso.Data.ApplicationContext NovaConexao() {
            return new Curso.Data.ApplicationContext();
        }

        static int _count = 0;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            _count = 0;
            using var db = NovaConexao();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __ ) => ++ _count; 

            if(gerenciarEstadoConexao){
                conexao.Open();
            }

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            } 

            time.Stop();
            var msg = $"Tempo: {time.Elapsed.ToString() }, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(msg);
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

        static void HealthCheckBancoDeDadosAntes(){
            using var db = new Curso.Data.ApplicationContext();

            try 
            {
                //1
                var connection = db.Database.GetDbConnection();
                connection.Open();

                //2
                db.Departamentos.Any();
                Console.WriteLine("Banco de dados funcionando");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Banco de dados não está funcionando");
            } 
        }
        static void HealthCheckBancoDeDadosAgora(){
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
                Console.WriteLine("Banco de dados funcionando");
            else
                Console.WriteLine("Banco de dados não está funcionando");
        }
    }
}
