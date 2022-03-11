using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src.Data;

namespace EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ApplicationContext();

            db.Database.EnsureCreated(); 
            
            db.Pessoas.Add(new src.Domain.Pessoa
            {
                Id=1,
                Nome = "TESTE",
                Telefone = "999999"
            });

            db.SaveChanges();
            
            var pessoas = db.Pessoas.ToList();
            foreach(var item in pessoas)
            {
                Console.WriteLine($"Nome: {item.Nome}");
            }
            
        }
    }
}
