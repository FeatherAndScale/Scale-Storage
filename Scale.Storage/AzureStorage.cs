using System;
using System.Collections.Specialized;
using Microsoft.WindowsAzure.Storage;
using Scale.Storage.Queue;

namespace Scale.Storage
{
    public abstract class AzureStorage
    {
        protected readonly NameValueCollection Settings;
        protected readonly CloudStorageAccount StorageAccount;
        
        /// <summary>
        /// Instantiates a new <see cref="AzureQueueStorage"/> service.
        /// </summary>
        protected internal AzureStorage(NameValueCollection settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            Settings = settings;

            string connectionString = Settings["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("StorageConnectionString was not found in settings.");

            StorageAccount = CloudStorageAccount.Parse(connectionString);
        }
    }
}
