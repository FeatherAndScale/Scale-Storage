using Newtonsoft.Json;

namespace Scale.Storage.Queue.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="QueueMessage"/>
    /// </summary>
    public static class QueueMessageExtensions
    {
        /// <summary>
        /// Deserialises the Data of the message to a model T.
        /// </summary>
        public static T GetModel<T>(this QueueMessage message)
        {
            return JsonConvert.DeserializeObject<T>(message.Data.ToString());
        }
    }
}
