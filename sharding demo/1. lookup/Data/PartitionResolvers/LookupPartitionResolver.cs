using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.PartitionResolvers
{
    internal class ShardMap
    {
        public string id { get; set; }
        [JsonProperty(PropertyName = "tenantCollectionMap")]
        public List<TenantCollection> TenantCollections { get; set; }
    }
    internal class TenantCollection
    {
        public string tenant { get; set; }
        [JsonProperty(PropertyName = "collectionLinks")]
        public List<CollectionMap> CollectionLinks { get; set; }
    }
    internal class CollectionMap
    {
        public bool? isActive { get; set; }
        public string collectionLink { get; set; }
        public Date dateMapped { get; set; }
    }
    internal class Date
    {
        public DateTime date { get; set; }
        public Int64 epoch { get; set; }
    }

    public class LookupPartitionResolver : IPartitionResolver 
    {
        private readonly string masterCollectionLink = "dbs/kYMpAA==/colls/kYMpAPEnjgA=/";
        private string databaseLink;
        private DocumentClient client;
        private ShardMap shardMap;

        public LookupPartitionResolver(string databaseLink)
        {
            this.databaseLink = databaseLink;
        }

        public object GetPartitionKey(string databaseLink, object document)
        {
            throw new NotImplementedException();
        }

        public string ResolveForCreate(string databaseLink, object partitionKey)
        {
            string tenant = (string)partitionKey;

            var link = (
                from tc in this.shardMap.TenantCollections
                from cl in tc.CollectionLinks
                where (tc.tenant == partitionKey.ToString()) && (cl.isActive == true)
                orderby cl.dateMapped descending                      
                select cl.collectionLink)
                .Single();
            
            return link;
        }

        public IEnumerable<string> ResolveForRead(string databaseLink, object partitionKey)
        {
            string tenant = (string)partitionKey;

            var links = (
                from tc in this.shardMap.TenantCollections
                from cl in tc.CollectionLinks
                where tc.tenant == partitionKey.ToString()
                select cl.collectionLink);

            return links;
        }

        public void Initialize(DocumentClient client)
        {
            this.client = client;
            this.shardMap = LoadShardMap();
        }

        private ShardMap LoadShardMap()
        {
            ShardMap shardMap = client.CreateDocumentQuery<ShardMap>(masterCollectionLink)
                                    .Where(d => d.id == "shardMap")
                                    .AsEnumerable()
                                    .Single();

            return shardMap;
        }
    }
}
