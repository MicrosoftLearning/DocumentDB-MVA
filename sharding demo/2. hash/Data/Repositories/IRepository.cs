using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IRepository<T> where T : class
    {
        Task<T> Create(string collectionLink, T entity);

        //Task Replace(string id, T entity);

        //Task Delete(string id);

        IEnumerable<T> Find(string collectionLink, Expression<Func<T, bool>> predicate);

        //IQueryable<T> FindAll();

        //T Get(string collectionLink, string entityId);
    }
}
