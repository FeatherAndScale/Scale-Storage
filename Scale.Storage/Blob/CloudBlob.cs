using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scale.Storage.Blob
{
    public class CloudBlob
    {
        public System.IO.Stream Stream { get; set; }
        public string Uri { get; set; }
        public string ContainerName { get; set; }
    }
}
