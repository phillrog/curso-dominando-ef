using System;
using Microsoft.EntityFrameworkCore;
using src.Data;

namespace EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ApplicationContext();

            //db.Database.Migrate();

            var migracoes = db.Database.GetPendingMigrations();
            foreach(var migracao in migracoes)
            {
                Console.WriteLine(migracao);
            }
            
            Console.WriteLine("Hello World!");
        }
    }
}
