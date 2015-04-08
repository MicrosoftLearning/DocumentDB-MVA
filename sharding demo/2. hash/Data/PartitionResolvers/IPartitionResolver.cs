using Microsoft.Azure.Documents.Client;

namespace Data.PartitionResolvers
{
    using System.Collections.Generic;


    public interface IPartitionResolver
    {
        // Extracts the partition key from a document
        object GetPartitionKey(string databaseLink, object document);

        // Given a database link and a partition key, returns the correct collection self-link for creating a document.
        string ResolveForCreate(string databaseLink, object partitionKey);

        // Given a database link and a partition key, returns a list of collection links to read from.
        IEnumerable<string> ResolveForRead(string databaseLink, object partitionKey);
    }
}
