using System;
using System.Collections.Generic;
using System.Linq;
using Curso.Domain;
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

           //CarregamentoAdiantado();
           //CarregamentoExplicito();
            //CarregamentoLento();

            //FiltroGlobal();
            //IgnoreFiltroGlobal();
            // ConsultaProjetada();
            //ConsultaParametrizada();
            //ConsultaInterpolada();
            //ConsultaComTAG();
            //EntendendoConsulta1NN1();
            //DivisaoDeConsulta();
            //CriarStoredProcedure();
            //InserirDadosViaProcedure();
            //CriarStoredProcedureDeConsulta();
            //ConsultaViaProcedure();
            //ConsultarDepartamentos();
            //DadosSensiveis();
            //TempoComandoGeral();
            //ExecutarEstrategiaResiliencia();
            //Collations();
            //PropagarDados();
            //Esquema();
            //ConversorDeValor();
            //ConversorCustomizado();
            //PropriedadesDeSombra();
            //TrabalhandoComPropriedadesDeSombra();
            //TiposDePropriedades();
            //Relacionamento1Para1();
            //Relacionamento1ParaMuitos();
            //RelacionamentoMuitosParaMuitos();
            //CampoDeApoio();
            //ExemploTPH();
            //PacotesDePropriedades();
            //Atributos();
            //FuncoesDeDatas();
            //FuncaoLike();
            //FuncaoDataLength();
        }

        static void FuncaoProperty()
        {
            ApagarCriarBancoDeDados();

            using (var db = NovaConexao())
            {
                var resultado = db
                    .Funcoes
                    //.AsNoTracking()
                    .FirstOrDefault(p=> EF.Property<string>(p, "PropriedadeSombra") == "Teste");

                var propriedadeSombra = db
                    .Entry(resultado)
                    .Property<string>("PropriedadeSombra")
                    .CurrentValue;

                Console.WriteLine("Resultado:");     
                Console.WriteLine(propriedadeSombra); 
            }
        } 

        static void FuncaoDataLength()
        {
            using (var db = NovaConexao())
            {
                var resultado = db
                    .Funcoes
                    .AsNoTracking()
                    .Select(p => new 
                    {
                        TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
                        TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
                        TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
                        Total1 = p.Descricao1.Length,
                        Total2 = p.Descricao2.Length
                    })
                    .FirstOrDefault();

                Console.WriteLine("Resultado:"); 

                Console.WriteLine(resultado); 
            }
        }
        
        static void FuncaoLike()
        {
            using (var db = NovaConexao())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db
                    .Funcoes
                    .AsNoTracking()
                    //.Where(p=> EF.Functions.Like(p.Descricao1, "Bo%"))
                    .Where(p=> EF.Functions.Like(p.Descricao1, "B[ao]%"))
                    .Select(p => p.Descricao1)
                    .ToArray();

                Console.WriteLine("Resultado:");
                foreach (var descricao in dados)
                {
                    Console.WriteLine(descricao);
                }
            }
        }
        static void FuncoesDeDatas()
        {
            ApagarCriarBancoDeDados();

            using (var db = NovaConexao())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db.Funcoes.AsNoTracking().Select(p =>
                   new
                   {
                       Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
                       Meses = EF.Functions.DateDiffMonth(DateTime.Now, p.Data1),
                       Data = EF.Functions.DateFromParts(2021, 1, 2),
                       DataValida = EF.Functions.IsDate(p.Data2),
                   });

                foreach (var f in dados)
                {
                    Console.WriteLine(f);
                }

            }
        }
        static void ApagarCriarBancoDeDados()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Funcoes.AddRange(
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(2),
                Data2 = "2021-01-01",
                Descricao1 = "Bala 1 ",
                Descricao2 = "Bala 1 "
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Bola 2",
                Descricao2 = "Bola 2"
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Tela",
                Descricao2 = "Tela"
            });

            db.SaveChanges();
        }

        static void Atributos()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                db.Atributos.Add(new Atributo
                {
                    Descricao = "Exemplo",
                    Observacao = "Observacao"
                });

                db.SaveChanges();
            }
        }

        static void PacotesDePropriedades()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var configuracao = new Dictionary<string, object>
                {
                    ["Chave"] = "SenhaBancoDeDados",
                    ["Valor"] = Guid.NewGuid().ToString()
                };

                db.Configuracoes.Add(configuracao);
                db.SaveChanges();

                var configuracoes = db
                    .Configuracoes
                    .AsNoTracking()
                    .Where(p => p["Chave"] == "SenhaBancoDeDados")
                    .ToArray();

                foreach (var dic in configuracoes)
                {
                    Console.WriteLine($"Chave: {dic["Chave"]} - Valor: {dic["Valor"]}");
                }
            }
        }

        static void ExemploTPH()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var pessoa = new Pessoa { Nome = "Fulano de Tal" };

                var instrutor = new Instrutor { Nome = "Rafael Almeida", Tecnologia = ".NET", Desde = DateTime.Now };

                var aluno = new Aluno { Nome = "Maria Thysbe", Idade = 31, DataContrato = DateTime.Now.AddDays(-1) };

                db.AddRange(pessoa, instrutor, aluno);
                db.SaveChanges();

                var pessoas = db.Pessoas.AsNoTracking().ToArray();
                var instrutores = db.Instrutores.AsNoTracking().ToArray();
                //var alunos = db.Alunos.AsNoTracking().ToArray();
                var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

                Console.WriteLine("Pessoas **************");
                foreach (var p in pessoas)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}");
                }

                Console.WriteLine("Instrutores **************");
                foreach (var p in instrutores)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Tecnologia: {p.Tecnologia}, Desde: {p.Desde}");
                }

                Console.WriteLine("Alunos **************");
                foreach (var p in alunos)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Idade: {p.Idade}, Data do Contrato: {p.DataContrato}");
                }
            }
        }

        static void CampoDeApoio()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var documento = new Documento();
                documento.SetCPF("12345678933");

                db.Documentos.Add(documento);
                db.SaveChanges();

                foreach (var doc in db.Documentos.AsNoTracking())
                {
                    Console.WriteLine($"CPF -> {doc.GetCPF()}");
                }
            }
        }

        static void RelacionamentoMuitosParaMuitos()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var ator1 = new Ator { Nome = "Rafael" };
                var ator2 = new Ator { Nome = "Pires" };
                var ator3 = new Ator { Nome = "Bruno" };

                var filme1 = new Filme { Descricao = "A volta dos que não foram" };
                var filme2 = new Filme { Descricao = "De volta para o futuro" };
                var filme3 = new Filme { Descricao = "Poeira em alto mar filme" };

                ator1.Filmes.Add(filme1);
                ator1.Filmes.Add(filme2);

                ator2.Filmes.Add(filme1);

                filme3.Atores.Add(ator1);
                filme3.Atores.Add(ator2);
                filme3.Atores.Add(ator3);

                db.AddRange(ator1, ator2, filme3);

                db.SaveChanges();

                foreach (var ator in db.Atores.Include(e => e.Filmes))
                {
                    Console.WriteLine($"Ator: {ator.Nome}");

                    foreach (var filme in ator.Filmes)
                    {
                        Console.WriteLine($"\tFilme: {filme.Descricao}");
                    }
                }
            }
        }

        static void Relacionamento1ParaMuitos()
        {
            using (var db = NovaConexao())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var estado = new Estado
                {
                    Nome = "Sergipe",
                    Governador = new Governador { Nome = "Rafael Almeida" }
                };

                estado.Cidades.Add(new Cidade { Nome = "Itabaiana" });

                db.Estados.Add(estado);

                db.SaveChanges();
            }

            using (var db = new Curso.Data.ApplicationContext())
            {
                var estados = db.Estados.ToList();

                estados[0].Cidades.Add(new Cidade { Nome = "Aracaju" });

                db.SaveChanges();

                foreach (var est in db.Estados.Include(p => p.Cidades).AsNoTracking())
                {
                    Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");

                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\t Cidade: {cidade.Nome}");
                    }
                }
            }
        }

        static void Relacionamento1Para1()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "Sergipe",
                Governador = new Governador { Nome = "Rafael Almeida" }
            };

            db.Estados.Add(estado);

            db.SaveChanges();

            var estados = db.Estados.Include(d=> d.Governador).AsNoTracking().ToList();

            estados.ForEach(est =>
            {
                Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");
            });
        }

        static void TiposDePropriedades()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente
            {
                Nome = "Fulano de tal",
                Telefone = "(79) 98888-9999",
                Endereco = new Endereco { Bairro = "Centro", Cidade = "Sao Paulo" }
            };

            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

                Console.WriteLine(json);
            });
        }

        static void TrabalhandoComPropriedadesDeSombra()
        {
            using var db = NovaConexao();
            /*db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento
            {
                Descricao = "Departamento Propriedade de Sombra"
            };

            db.Departamentos.Add(departamento);

            db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            db.SaveChanges();
            */

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now).ToArray();
        }
        static void PropriedadesDeSombra()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void ConversorCustomizado()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Conversores.Add(
                new Conversor
                {
                    Status = Status.Devolvido,
                });

            db.SaveChanges();

            var conversorEmAnalise = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Analise);

            var conversorDevolvido = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Devolvido);
        }

        static void ConversorDeValor() => Esquema();
        static void Esquema()
        {
            using var db = NovaConexao();

            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void PropagarDados()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void Collations()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
        static void ExecutarEstrategiaResiliencia()
        {
            using var db = NovaConexao();

            var strategy = db.Database.CreateExecutionStrategy();
            strategy.Execute(()=>
            {
                using var transaction = db.Database.BeginTransaction();

                db.Departamentos.Add(new Departamento { Descricao = "Departamento Transacao"});
                db.SaveChanges();

                transaction.Commit();
            });

        }

        static void TempoComandoGeral()
        {
            using var db = NovaConexao();

            db.Database.SetCommandTimeout(10);

            db.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:07';SELECT 1");
        }

        static void HabilitandoBatchSize()
        {
            using var db = NovaConexao();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (var i = 0; i < 50; i++)
            {
                db.Departamentos.Add(
                    new Departamento
                    {
                        Descricao = "Departamento " + i
                    });
            }

            db.SaveChanges();
        }

        static void DadosSensiveis()
        {
            using var db = NovaConexao();

            var descricao = "Departamento";
            var departamentos = db.Departamentos.Where(p => p.Descricao == descricao).ToArray();
        }

        static void ConsultarDepartamentos()
        {
            using var db = NovaConexao();

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToArray();
        }

        static void ConsultaViaProcedure()
        {
            using var db = NovaConexao();

            var dep = new SqlParameter("@dep", "Departamento");

            var departamentos = db.Departamentos
                //.FromSqlRaw("EXECUTE GetDepartamentos @dep", dep)
                .FromSqlInterpolated($"EXECUTE GetDepartamentos {dep}")
                .ToList();

            foreach(var departamento in departamentos)
            {
                Console.WriteLine(departamento.Descricao);
            }
        }

        static void InserirDadosViaProcedure()
        {
            using var db = NovaConexao();

            db.Database.ExecuteSqlRaw("execute CriarDepartamento @p0, @p1", "Departamento Via Procedure", true);
        }

        static void CriarStoredProcedure()
        {
            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE CriarDepartamento
                @Descricao VARCHAR(50),
                @Ativo bit
            AS
            BEGIN
                INSERT INTO 
                    Departamentos(Descricao, Ativo, Excluido) 
                VALUES (@Descricao, @Ativo, 0)
            END        
            ";
            
            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }

        static void CriarStoredProcedureDeConsulta()
        {
            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE GetDepartamentos
                @Descricao VARCHAR(50)
            AS
            BEGIN
                SELECT * FROM Departamentos Where Descricao Like @Descricao + '%'
            END        
            ";
            
            using var db = NovaConexao();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }
        static void DivisaoDeConsulta()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .Where(p => p.Id < 3)
                //.AsSplitQuery()
                .AsSingleQuery()
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }
        }
        static void EntendendoConsulta1NN1()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var funcionarios = db.Funcionarios
                .Include(p => p.Departamento)
                .ToList();


            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"Nome: {funcionario.Nome} / Descricap Dep: {funcionario.Departamento.Descricao}");
            }

            /*var departamentos = db.Departamentos
                .Include(p=>p.Funcionarios)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }*/
        }
        static void ConsultaComTAG()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .TagWith(@"Enviando um comentario para o servidor
                
                Segundo comentario
                Terceiro comentario
                Quarto comentario")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
 
        static void ConsultaInterpolada()
        {
            using var db = NovaConexao();
            Setup(db);

            var id = 1;
            var departamentos = db.Departamentos
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id>{id}")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaParametrizada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var id = new SqlParameter
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };
            var departamentos = db.Departamentos
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id>{0}", id)
                .Where(p => !p.Excluido)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaProjetada()
        {
            using var db = NovaConexao();
            Setup(db);

            var departamentos = db.Departamentos
                .Where(p => p.Id > 0)
                .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }
        static void IgnoreFiltroGlobal()
        {
            using var db = NovaConexao();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void FiltroGlobal()
        {
            using var db = NovaConexao();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void Setup(Curso.Data.ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Bruno Brito",
                                CPF = "88888888811",
                                RG= "3100062"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG= "1100062"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void CarregamentoLento()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db
                .Departamentos
                .ToList();

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
        static void CarregamentoExplicito()
        {
            using var db = NovaConexao();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                if(departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection(p=>p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p=>p.Funcionarios).Query().Where(p=>p.Id > 2).ToList();
                }

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
