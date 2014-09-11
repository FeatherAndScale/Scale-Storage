using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Scale.Storage.Table
{
    /// <summary>
    /// Defines a Table Storage service class.
    /// </summary>
    public interface ITableStorage
    {
        /// <summary>
        /// Creates tables for tableNames if they don't already exist.
        /// </summary>
        Task CreateTables(string[] tableNames);

        /// <summary>
        /// Inserts entity into tableName.
        /// </summary>
        Task Insert(string tableName, TableEntity entity);

        /// <summary>
        /// Inserts entities into tableName.
        /// </summary>
        Task Insert(string tableName, TableEntity[] entities);

        /// <summary>
        /// Retrieves an entity of T for a given table, partitionKey and rowKey. T can be any <see cref="TableEntity"/>
        /// </summary>
        Task<T> Retrieve<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity;
    }
}
