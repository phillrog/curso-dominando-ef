﻿using System;
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
            //SqlInjection() //5
            //MigracoesPendentes(); //6
            // AplicarMigracaoEmTempoDeExecucao(); //7
            //TodasAsMigracoes(); //8
           // MigracoesJaAplicadas(); 9

           //ScriptGeralDoBancoDeDados();

           CarregamentoAdiantado();
        }

        static void CarregamentoAdiantado()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {

                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void SetupTiposCarregamentos(Curso.Data.ApplicationContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Teste 01",
                                CPF = "99999999999",
                                RG= "00000000"
                            }
                        }
                    },
                    new Curso.Domain.Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Tese 02",
                                CPF = "111111111",
                                RG= "3333333"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Teste 03",
                                CPF = "33333333",
                                RG= "77777777"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        private static void ScriptGeralDoBancoDeDados()
        {
            using var db = NovaConexao();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        private static void MigracoesJaAplicadas()
        {
            using var db = NovaConexao();
            var migracoes = db.Database.GetAppliedMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        private static void TodasAsMigracoes()
        {
            using var db = NovaConexao();
            var migracoes = db.Database.GetMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        /// Não é uma boa pratica
        private static void AplicarMigracaoEmTempoDeExecucao()
        {
           using var db = NovaConexao();

           db.Database.Migrate(); 
        }

        /// Detectar migrações que não foram executadas
        static void MigracoesPendentes(){
             using var db = NovaConexao();

             var migracoesPendentes = db.Database.GetPendingMigrations();
             Console.WriteLine($"Total: {migracoesPendentes.Count()}");
             foreach (var migracao in migracoesPendentes)
             {
                 Console.WriteLine($"Migração: {migracao}");
             }
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
