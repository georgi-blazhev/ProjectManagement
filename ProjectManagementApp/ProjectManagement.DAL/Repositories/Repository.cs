using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : AbstractEntity
    {
        private readonly DatabaseContext _databaseContext;

        public Repository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public Task<List<T>> All()
        {
            return _databaseContext.Set<T>().ToListAsync();
        }

        public async System.Threading.Tasks.Task Create(T entity)
        {
            await _databaseContext.AddAsync(entity);
            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task Delete(T entity)
        {
            _databaseContext.Remove(entity);
            await _databaseContext.SaveChangesAsync();
        }

        public List<T> Find(Func<T, bool> predicate)
        {
            return _databaseContext.Set<T>().Where(predicate).ToList();
        }

        public Task<T> Get(int id)
        {
            return _databaseContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public T Get(Func<T, bool> predicate)
        {
            return _databaseContext.Set<T>().FirstOrDefault(predicate);
        }

        public async System.Threading.Tasks.Task Update()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
