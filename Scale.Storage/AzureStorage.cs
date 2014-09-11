using System;
using System.Collections.Specialized;
using Microsoft.WindowsAzure.Storage;
using Scale.Storage.Queue;

namespace Scale.Storage
{
    /// <summary>
    /// Azure Storage base class.
    /// </summary>
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

            string connectionString = Settings["AZURE_STORAGE_CONNECTION_STRING"];
            if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING was not found in settings.");

            StorageAccount = CloudStorageAccount.Parse(connectionString);
        }
    }
}
