using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestDbSet<T> : IEnumerable<T>, IQueryable<T>
    {
        private List<T> _entities;

        public Expression Expression => this.AsQueryable().Expression;

        public Type ElementType => this.AsQueryable().ElementType;

        public IQueryProvider Provider => this.AsQueryable().Provider;

        public IEnumerator<T> GetEnumerator() => _entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(IEnumerable<T> itemsToAdd) => _entities.AddRange(itemsToAdd);

        public void Add(T itemToAdd) => _entities.Add(itemToAdd);

        public void Remove(T itemToRemove) => _entities.Remove(itemToRemove);

        public T SingleOrDefault(Func<T, bool> predicate) => _entities.SingleOrDefault(predicate);

        public async Task<T> SingleOrDefaultAsync(Func<T, bool> predicate) => SingleOrDefault(predicate);

        public List<T> ToList() => _entities.ToList();

        public async Task<List<T>> ToListAsync() => _entities.ToList();

        public IEnumerable<T> Skip(int count) => _entities.Skip(count);

        public IEnumerable<T> Where(Func<T, bool> predicate) => _entities.Where(predicate);

        public bool Any(Func<T, bool> predicate) => _entities.Any(predicate);

        public T Find(Predicate<T> match) => _entities.Find(match);

        public async Task<T> FindAsync(Predicate<T> match) => Find(match);
    }
}
