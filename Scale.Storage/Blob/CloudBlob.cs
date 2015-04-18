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

        public override string ToString()
        {
            return base.ToString() + string.Format("{{ContainerName={0}, Uri={1}}}", ContainerName, Uri);
        }
    }
}
