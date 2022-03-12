using System.Collections.Generic;

namespace src.Domain
{
    public class Departamento
    {
        public int Id {get;set;}
        public string Descricao{get;set;}

        public List<Colaborador> Colaboradores {get;set;}
    }
}