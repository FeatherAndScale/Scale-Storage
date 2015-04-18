using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scale.Storage.Blob
{
    public interface IBlobStorage
    {
        Task CreateContainer(string containerName);

        Task<string> Upload(string containerName, string blobName, string contentType, Stream stream);

		Task<CloudBlob> Download(string uri);

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="folderName">The folder name of the Blobs to list.</param>
        /// <param name="pages">The maximum number of Pages of items to retrieve. There are 5000 items per Page (this is the API limit).</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        Task<IEnumerable<CloudBlob>> List(string containerName, string folderName, int pages);

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="flatten">When true, will return a flattened list of all Blobs in the container and sub-folders.</param>
        /// <param name="pages">The maximum number of Pages of items to retrieve. There are 5000 items per Page (this is the API limit).</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        Task<IEnumerable<CloudBlob>> List(string containerName, bool flatten, int pages);

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="folderName">The folder name of the Blobs to list.</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        /// <remarks>Will only return one page of max 5000 items (this is the API limit). See other overloads if you need more pages.</remarks>
        Task<IEnumerable<CloudBlob>> List(string containerName, string folderName);

        /// <summary>
        /// Gets a list of <see cref="CloudBlob" for a given containerName and folderName./>
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="flatten">When true, will return a flattened list of all Blobs in the container and sub-folders.</param>
        /// <returns>Task of IEnumerable of <see cref="CloudBlob"/>. Streams are not set on these results, use <see cref="Download"/> to get Streams.</returns>
        /// <remarks>Will only return one page of max 5000 items (this is the API limit). See other overloads if you need more pages.</remarks>
        Task<IEnumerable<CloudBlob>> List(string containerName, bool flatten);
    }
}
