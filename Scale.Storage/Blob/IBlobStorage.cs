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
    }
}
