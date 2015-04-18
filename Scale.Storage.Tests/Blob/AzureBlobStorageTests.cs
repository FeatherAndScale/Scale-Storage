using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scale.Storage.Blob;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Scale.Storage.Tests.Blob
{
    [TestClass]
    public class AzureBlobStorageTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("Slow")]
        public async Task List_Flatten_ReturnsItemsInFolders()
        {
            // setup
            var settings = new NameValueCollection();
            settings.Add("AZURE_STORAGE_CONNECTION_STRING", Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? "");

            var storage = new AzureBlobStorage(settings);
            string guid = Guid.NewGuid().ToString("N");
            
            string containerName = "c_" + guid;
            await storage.CreateContainer(containerName);

            // upload 5 blobs
            var newBlobName = new Func<string>(() => "b_" + Guid.NewGuid().ToString("N"));
            string[] blobNames = new[] { newBlobName(), newBlobName(), newBlobName(), newBlobName(), newBlobName() };
            var stream = new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read);
            //Action upload = ()=> storage.Upload(containerName, newBlobName(), "image/jpeg", stream);
            //var tasks = new Task[]{new Task( upload), new Task( upload)};
            var tasks = new Task[] {storage.Upload(containerName, newBlobName(), "image/jpeg", stream), 
                storage.Upload(containerName, newBlobName(), "image/jpeg", stream),
                storage.Upload(containerName, newBlobName(), "image/jpeg", stream),
                storage.Upload(containerName, newBlobName(), "image/jpeg", stream),
                storage.Upload(containerName, newBlobName(), "image/jpeg", stream)};
            Task.WaitAll(tasks);
            
            try
            {            
                // arrange

                // act
                var blobs = await storage.List(containerName, true);

                // assert
                var uris = blobs.Select(b => b.Uri);
                Assert.IsTrue(uris.Count() == 5, "List should return 5 items.");
                string allUris = string.Join(",", uris);
                Trace.TraceInformation("allUris = " + allUris);
                Assert.IsTrue(blobNames.All(b => allUris.Contains(b)), 
                    "There should be a blob URI in the list for each if the Blobs uploaded at Setup.");
            }
            // teardown
            finally
            {
                //TODO: Delete container
                //TODO: Delete blob
            }
        }
    }
}
