using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Collections.Specialized;

namespace Scale.Storage.Blob
{
    public class AzureBlobStorage : AzureStorage, IBlobStorage
    {
        // http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/

        private CloudBlobClient _blobClient;

        /// <summary>
        /// Instantiates a new <see cref="AzureTableStorage"/>.
        /// </summary>
        /// <param name="settings"></param>
        public AzureBlobStorage(NameValueCollection settings)
            : base(settings)
        {
            _blobClient = StorageAccount.CreateCloudBlobClient();
        }

        public async Task CreateContainer(string containerName)
        {
            // Retrieve a reference to a container. 
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();
        }

        public async Task<string> Upload(string containerName, string blobName, string contentType, Stream stream)
        {
            // Retrieve reference to a previously created container.
            var container = _blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromStreamAsync(stream);
            return blockBlob.Uri.ToString();
        }

        public async Task<CloudBlob> Download(string uri)
        {
            var blob = await _blobClient.GetBlobReferenceFromServerAsync(new Uri(uri));

            // Download to memory stream
            var stream = new System.IO.MemoryStream();            
            await blob.DownloadToStreamAsync(stream);
            return new CloudBlob { Stream = stream, ContainerName = blob.Container.Name, Uri = blob.Uri.ToString() };
        }
    }
}
