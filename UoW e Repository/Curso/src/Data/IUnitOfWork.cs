using System;
using src.Data.Repositories;

namespace src.Data
{
    public interface IUnitOfWork : IDisposable
    {
         bool Commit();
         IDepartamentoRepository DepartamentoRepository{get;}
    }
}