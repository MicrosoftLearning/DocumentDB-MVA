using Microsoft.Azure.Documents.PartitionResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.PartitionResolvers
{
    public sealed class CustomHashPartitionResolverProperties : SpilloverPartitionResolverProperties
    {
        private const bool defaultSpillover = true;

        public CustomHashPartitionResolverProperties(string partitionPropertyPath, string namePrefix, int collectionCount )
            : base()
        {
            this.PartitionKeyPropertyPath = partitionPropertyPath;
            this.Spillover = defaultSpillover;
        }

        public string NamePrefix { get; set; }
        public int CollectionCount { get; set; }
        public bool Spillover { get; set; }
        public string PartitionKeyPropertyPath { get; set; }
    }
}
