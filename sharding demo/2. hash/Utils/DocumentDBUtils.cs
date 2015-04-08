using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class DocumentDBUtils
    {
        public static Database GetOrCreateDatabase(DocumentClient client, string databaseId)
        {
            IQueryable<Database> dbQuery = from db in client.CreateDatabaseQuery()
                                           where db.Id == databaseId
                                           select db;

            IEnumerable<Database> enumerable = dbQuery.AsEnumerable();
            if (!enumerable.Any())
            {
                return client.CreateDatabaseAsync(new Database() { Id = databaseId }).Result.Resource;
            }
            else
            {
                return enumerable.First<Database>();
            }
        }
        public static DocumentCollection GetOrCreateDocumentCollection(DocumentClient client, string databaseLink,
            string collectionId)
        {
            return GetOrCreateDocumentCollection(client, databaseLink, collectionId, indexingPolicy: null);
        }

        public static DocumentCollection GetOrCreateDocumentCollection(DocumentClient client, string databaseLink,
            string collectionId, IndexingPolicy indexingPolicy)
        {
            IQueryable<DocumentCollection> collectionQuery =
                from coll in client.CreateDocumentCollectionQuery(databaseLink)
                where coll.Id == collectionId
                select coll;

            IEnumerable<DocumentCollection> enumerable = collectionQuery.AsEnumerable();
            if (!enumerable.Any())
            {
                DocumentCollection collection = new DocumentCollection() { Id = collectionId };

                if (indexingPolicy != null)
                {
                    collection.IndexingPolicy.Automatic = indexingPolicy.Automatic;
                    collection.IndexingPolicy.IndexingMode = indexingPolicy.IndexingMode;

                    foreach (var path in indexingPolicy.IncludedPaths)
                    {
                        collection.IndexingPolicy.IncludedPaths.Add(path);
                    }

                    foreach (var path in indexingPolicy.ExcludedPaths)
                    {
                        collection.IndexingPolicy.ExcludedPaths.Add(path);
                    }
                }

                return client.CreateDocumentCollectionAsync(databaseLink, collection).Result.Resource;
            }
            else
            {
                return enumerable.First();
            }
        }
    }
}
