using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using src;
using src.Data;

namespace EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            DiagnosticListener.AllListeners.Subscribe(new MyInterceptorListener());
            
            using var db = new ApplicationContext();
            db.Database.EnsureCreated();

            //var sql = db.Departamentos.Where(p=>p.Id > 0).ToQueryString();

            _ = db.Departamentos.Where(p=>p.Id > 0).ToArray();

            //Console.WriteLine(sql);
        }
    }
}
