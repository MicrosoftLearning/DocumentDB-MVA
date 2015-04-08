using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using Models;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Dynamic;
using Data.Repositories;
using Utils;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        readonly string serviceEndpoint = "";
        readonly string authKey = "";
        readonly string databaseId = "mva";

        DocumentClient client;
        Database database;

        [TestInitialize]
        public void InitDb()
        {
            this.client = client = new DocumentClient(new Uri(this.serviceEndpoint), this.authKey);
            this.database = DocumentDBUtils.GetOrCreateDatabase(this.client, this.databaseId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //client.DeleteDatabaseAsync(this.database.SelfLink);
            this.client.Dispose();
        }

        [TestMethod]
        public void CanWriteToDb()
        {
            string tenantId = "Contoso Inc.";
            string id = Guid.NewGuid().ToString();
            string desc = "do something awesome";

            var doc = new Item { Id = id, Description = desc };

            ItemRepository repo = new ItemRepository(this.client, this.database.Id);
            Item created = repo.Create(doc, tenantId).Result;
            Assert.IsNotNull(created);
            Assert.AreEqual(created.Description, desc);

            tenantId = "Litware Inc.";
            id = Guid.NewGuid().ToString();
            desc = "do something awesome";

            doc = new Item { Id = id, Description = desc };

            created = repo.Create(doc, tenantId).Result;
            Assert.IsNotNull(created);
            Assert.AreEqual(created.Description, desc);

            tenantId = "Adventure Works";
            id = Guid.NewGuid().ToString();
            desc = "do something awesome";

            doc = new Item { Id = id, Description = desc };

            created = repo.Create(doc, tenantId).Result;
            Assert.IsNotNull(created);
            Assert.AreEqual(created.Description, desc);
        }

        [TestMethod]
        public void CanReadFromDb()
        {
            string tenantId = "Contoso Inc.";
            string id = Guid.NewGuid().ToString();
            string desc = "do something awesome";

            var doc = new Item { Id = id, Description = desc };

            ItemRepository repo = new ItemRepository(this.client, this.database.Id);
            Item created = repo.Create(doc, tenantId).Result;

            Item read = repo.Find(tenantId, item => item.Id == id).FirstOrDefault();

            Assert.IsNotNull(read);
            Assert.AreEqual(created.Id, read.Id);
            Assert.AreEqual(created.Description, read.Description);
        }
    }
}