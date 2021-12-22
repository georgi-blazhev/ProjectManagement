using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Abstraction
{
    public interface IRepository<T> where T : AbstractEntity
    {
        System.Threading.Tasks.Task Create(T entity);
        System.Threading.Tasks.Task Delete(T entity);
        System.Threading.Tasks.Task Update();
        Task<T> Get(int id);
        T Get(Func<T, bool> predicate);
        Task<List<T>> All();
        List<T> Find(Func<T, bool> predicate);
    }
}
