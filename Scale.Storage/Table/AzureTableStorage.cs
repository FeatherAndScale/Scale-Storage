using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Scale.Storage.Table
{
    /// <summary>
    /// An Azure Table Storage service class.
    /// </summary>
    public class AzureTableStorage : AzureStorage, ITableStorage
    {
        private readonly CloudTableClient _tableClient; 

        /// <summary>
        /// Instantiates a new <see cref="AzureTableStorage"/>.
        /// </summary>
        /// <param name="settings"></param>
        public AzureTableStorage(NameValueCollection settings) : base(settings)
        {
            _tableClient = StorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates tables for tableNames if they don't already exist.
        /// </summary>
        public async Task CreateTables(string[] tableNames)
        {
            if (tableNames == null || tableNames.Length == 0) throw new ArgumentNullException("tableNames");
            foreach (string tableName in tableNames.AsParallel())   //REVIEW
            {
                var table = _tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();
                Trace.TraceInformation("Created table " + tableName);
            }
        }

        /// <summary>
        /// Inserts entity into tableName.
        /// </summary>
        public async Task Insert(string tableName, TableEntity entity)
        {
            var table = _tableClient.GetTableReference(tableName);
            var insertOperation = TableOperation.Insert(entity);
            var result = await table.ExecuteAsync(insertOperation);
            Trace.TraceInformation("Inserted entity {0}, result = {1}", entity, result);
            //TODO: Handle error here?
        }

        /// <summary>
        /// Inserts entities into tableName.
        /// </summary>
        public async Task Insert(string tableName, TableEntity[] entities)
        {
            var table = _tableClient.GetTableReference(tableName);
            var insertOperation = new TableBatchOperation();
            entities.ToList().ForEach(insertOperation.Insert);
            var results = await table.ExecuteBatchAsync(insertOperation);
            Trace.TraceInformation("Inserted entities {0}, results = {1}", string.Join(",", entities.ToString()),
                string.Join(",", results.ToString()));
            //TODO: Handle errors here?
            //TODO: Return (abstracted) results?
        }

        /// <summary>
        /// Retrieves an entity of T for a given table, partitionKey and rowKey. T can be any <see cref="TableEntity"/>
        /// </summary>
        public async Task<T> Retrieve<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity
        {
            var table = _tableClient.GetTableReference(tableName);
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);
            return result.Result as T;
        }

        /// <summary>
        /// Retrieves an entity of T for a given table, partitionKey and rowKey. T can be any <see cref="TableEntity"/>
        /// </summary>
        public async Task<IEnumerable<T>> Query<T>(string tableName, TableQuery<T> query) where T : TableEntity, new()
        {
            var table = _tableClient.GetTableReference(tableName);
            TableContinuationToken token = null;
            var entities = new List<T>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        /// <summary>
        /// Retrieves an entity of T for a given table, partitionKey and rowKey. T can be any <see cref="TableEntity"/>
        /// </summary>
        public async Task<IEnumerable<DynamicTableEntity>> Query(string tableName, TableQuery query) 
        {
            var table = _tableClient.GetTableReference(tableName);
            TableContinuationToken token = null;
            var entities = new List<DynamicTableEntity>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        /// <summary>
        /// Gets an instance of an <see cref="AzureTableStorage"/>.
        /// </summary>
        /// <param name="settings">A <see cref="NameValueCollection"/> of settings for the application. <see cref="AzureTableStorage"/> expects
        ///  a value for the key "StorageConnectionString" to be present.</param>
        [Obsolete("Use public constructor on Instance")]
        public static AzureTableStorage GetTableStorage(NameValueCollection settings)
        {
            return new AzureTableStorage(settings);
        }
    }
}
