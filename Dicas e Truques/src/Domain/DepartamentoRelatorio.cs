using System;
using Microsoft.EntityFrameworkCore;

namespace src.Domain
{
    public class DepartamentoRelatorio
    {
        public string Departamento { get; set; }
        public int Colaboradores { get; set; }
    }
}