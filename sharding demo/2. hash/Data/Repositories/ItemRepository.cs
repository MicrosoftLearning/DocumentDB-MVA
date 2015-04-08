using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.PartitionResolvers;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public class ItemRepository : DocumentDBRepository<Item>
    {
        private HashPartitionResolver partitionResolver;
        
        public ItemRepository(DocumentClient client, string databaseId) : base(client, databaseId) 
        {
            this.partitionResolver = new HashPartitionResolver(base.database.SelfLink);
            this.partitionResolver.Initialize(client);
        }

        public IEnumerable<Item> Find(Expression<Func<Item, bool>> predicate) 
        { 
            return this.Find(null, predicate); 
        }

        public override IEnumerable<Item> Find(string tenantId, Expression<Func<Item, bool>> predicate)
        {
            IEnumerable<string> collectionLinks = partitionResolver.ResolveForRead(base.database.SelfLink, tenantId);
            return collectionLinks.SelectMany(collectionLink => base.Find(collectionLink, predicate)).ToList();
        }
        
        public async Task<Item> Create(Item item, string tenantId)
        {
            string collectionLink = partitionResolver.ResolveForCreate(base.database.SelfLink, tenantId);
            item.TenantId = tenantId;

            return (Item) await base.Create(collectionLink, item);
        }
    }
}
