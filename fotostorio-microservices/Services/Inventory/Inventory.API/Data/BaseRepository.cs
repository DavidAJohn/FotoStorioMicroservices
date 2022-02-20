using Inventory.API.Contracts;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Inventory.API.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly InventoryDbContext _context;

        public BaseRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> ListAllAsync()
        {
            return await _context.Set<T>()
                .ToListAsync();
        }

        public async Task<T> Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await Save();
            return entity;
        }

        public async Task<bool> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return await Save();
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await Save();
        }

        private async Task<bool> Save()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
