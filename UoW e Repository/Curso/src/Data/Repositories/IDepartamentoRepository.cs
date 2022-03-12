using System.Threading.Tasks;
using src.Data.Repositories.Base;
using src.Domain;

namespace src.Data.Repositories
{
    public interface IDepartamentoRepository : IGenericRepository<Departamento>
    {
         //Task<Departamento> GetByIdAsync(int id); 
         //void Add(Departamento departamento);
         //bool Save();
    }
}