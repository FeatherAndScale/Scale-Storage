using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scale.Storage.Blob;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections.Generic;

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
            
            string containerName = "c" + guid;
            await storage.CreateContainer(containerName);

            // upload 5 blobs
            var blobNames = new List<string>(5);
            var newBlobName = new Func<string>(() => { 
                string name = "B" + Guid.NewGuid().ToString("N") + ".jpg";
                blobNames.Add(name);
                return name;
            });
            //string[] blobNames = new[] { newBlobName(), newBlobName(), newBlobName(), newBlobName(), newBlobName() };
            //var stream = new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read);
            //Action upload = ()=> storage.Upload(containerName, newBlobName(), "image/jpeg", stream);
            //var tasks = new Task[]{new Task( upload), new Task( upload)};
            var tasks = new Task[] {
                storage.Upload(containerName, newBlobName(), "image/jpeg", new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read)), 
                storage.Upload(containerName, newBlobName(), "image/jpeg", new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read)),
                storage.Upload(containerName, newBlobName(), "image/jpeg", new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read)),
                storage.Upload(containerName, newBlobName(), "image/jpeg", new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read)),
                storage.Upload(containerName, newBlobName(), "image/jpeg", new FileStream(@".\Blob\Test.jpg", FileMode.Open, FileAccess.Read))
            };
            Task.WaitAll(tasks);
            
            try
            {            
                // arrange

                // act
                var blobs = await storage.List(containerName, true);
                blobs.ToList().ForEach(b => Trace.TraceInformation(b.ToString()));

                // assert
                var uris = blobs.Select(b => b.Uri);
                Assert.IsTrue(uris.Count() == 5, "List should return 5 items.");
                string allUris = string.Join(",", uris);
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
