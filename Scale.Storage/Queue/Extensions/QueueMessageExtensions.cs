using Newtonsoft.Json;

namespace Scale.Storage.Queue.Extensions
{
    public static class QueueMessageExtensions
    {
        public static T GetModel<T>(this QueueMessage message)
        {
            return JsonConvert.DeserializeObject<T>(message.Data.ToString());
        }
    }
}
