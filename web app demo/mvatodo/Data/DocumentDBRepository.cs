using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace mvatodo.Data
{
    public class DocumentDBRepository<T> where T : class
    {
        public static async Task DeleteItemAsync(string id)
        {
            Document doc = GetDocument(id);
            await Client.DeleteDocumentAsync(doc.SelfLink);
        }

        public static T GetItem(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(CollectionLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .SingleOrDefault();
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            Document doc = GetDocument(id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }

        private static Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(CollectionLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .SingleOrDefault();
        } 

        public static async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(CollectionLink, item);
        }

        public static IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(CollectionLink)
                .Where(predicate)
                .AsEnumerable();
        }

        private static string collectionLink;
        private static string CollectionLink
        {
            get
            {
                if (collectionLink == null)
                {
                    collectionLink = ConfigurationManager.AppSettings["collectionLink"];
                }

                return collectionLink;
            }
        }

        private static DocumentClient client;
        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }
    }
}
