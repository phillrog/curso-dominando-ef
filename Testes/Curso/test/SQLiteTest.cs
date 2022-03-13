using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Entities;
using Xunit;

namespace EFCore.Testes
{
    public class SQLiteTest
    {
        [Theory]
        [InlineData("Tecnologia")]
        [InlineData("Financeiro")]
        [InlineData("Departamento Pessoal")]
        public void Deve_inserir_e_consultar_um_departamento(string descricao)
        {
            // Arrange
            var departamento = new Departamento
            {
                Descricao = descricao,
                DataCadastro = DateTime.Now
            };

            // Setup
            var context = CreateContext();
            context.Database.EnsureCreated();
            context.Departamentos.Add(departamento);

            // Act
            var inseridos = context.SaveChanges();
            departamento = context.Departamentos.FirstOrDefault(p=>p.Descricao == descricao);

            // Assert
            Assert.Equal(1, inseridos);
            Assert.Equal(descricao, departamento.Descricao);
        }

        private ApplicationContext CreateContext()
        {
            var conexao = new SqliteConnection("Datasource=:memory:");
            conexao.Open();

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                //.UseSqlite("Datasource=:memory:")
                .UseSqlite(conexao)
                .Options;

            return new ApplicationContext(options);    
        }
    }
}
