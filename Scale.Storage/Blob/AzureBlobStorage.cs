using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Scale.Storage.Blob
{
    /// <summary>
    /// Azure Block Blob Storage library wrapper for .NET. 
    /// </summary>
    /// <remarks>Only Block Storage is currently supported. See https://msdn.microsoft.com/en-us/library/azure/ee691964.aspx. </remarks>
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

            // Retrieve reference to blob.
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

        private async Task<IEnumerable<CloudBlob>> List(string containerName, string folderName, bool flatten, int pages)
        {
            // https://ahmetalpbalkan.com/blog/azure-listblobssegmentedasync-listcontainerssegmentedasync-how-to/

            // checks
            if (pages < 1) throw new ArgumentOutOfRangeException("pages");

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();

            for (int i = 0; i < pages; i++)
            {
                var response = await container.ListBlobsSegmentedAsync(folderName, flatten, BlobListingDetails.All, null, continuationToken, null, null);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results.Where(b=>b.GetType() == typeof(CloudBlockBlob)));
                if (continuationToken == null) break;
            }

            return results.Select(b => new CloudBlob { ContainerName = containerName, Uri = b.Uri.ToString() });
        }

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="folderName">The folder name of the Blobs to list.</param>
        /// <param name="pages">The maximum number of Pages of items to retrieve. There are 5000 items per Page (this is the API limit).</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        public async Task<IEnumerable<CloudBlob>> List(string containerName, string folderName, int pages)
        {
            return await List(containerName, folderName, false, pages);
        }

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="flatten">When true, will return a flattened list of all Blobs in the container and sub-folders.</param>
        /// <param name="pages">The maximum number of Pages of items to retrieve. There are 5000 items per Page (this is the API limit).</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        public async Task<IEnumerable<CloudBlob>> List(string containerName, bool flatten, int pages)
        {
            return await List(containerName, null, flatten, pages);
        }

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="folderName">The folder name of the Blobs to list.</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        /// <remarks>Will only return one page of max 5000 items (this is the API limit). See other overloads if you need more pages.</remarks>
        public async Task<IEnumerable<CloudBlob>> List(string containerName, string folderName)
        {
            return await List(containerName, folderName, false, 1);
        }

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="flatten">When true, will return a flattened list of all Blobs in the container and sub-folders.</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        /// <remarks>Will only return one page of max 5000 items (this is the API limit). See other overloads if you need more pages.</remarks>
        public async Task<IEnumerable<CloudBlob>> List(string containerName, bool flatten)
        {
            return await List(containerName, null, flatten, 1);
        }
    }
}
