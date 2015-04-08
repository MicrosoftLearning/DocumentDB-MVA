using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Data.Repositories
{
    public abstract class DocumentDBRepository<T> : IDisposable, IRepository<T> where T : Resource
    {
        bool disposed = false;
        protected DocumentClient client;
        protected Database database;

        protected DocumentDBRepository(DocumentClient client, string databaseId) 
        {
            this.client = client;
            this.database = DocumentDBUtils.GetOrCreateDatabase(client, databaseId);
        }

        public virtual IEnumerable<T> Find(string collectionLink, Expression<Func<T, bool>> predicate) 
        {
            var ret = client.CreateDocumentQuery<T>(collectionLink)
                .Where(predicate)
                .AsEnumerable();

            return ret;
        }

        public virtual async Task<T> Create(string collectionLink, T entity)
        {
            Document doc = await client.CreateDocumentAsync(collectionLink, entity);
            T ret = (T)(dynamic)doc;
            return ret;
        }

        public virtual T Get(string collectionLink, string documentId)
        {
            T doc = client.CreateDocumentQuery<T>(collectionLink)
                .Where(d => d.Id == documentId)
                .AsEnumerable()
                .FirstOrDefault();

            return doc;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                client.Dispose();
            }

            disposed = true;
        }
    }
}