using Newtonsoft.Json;

namespace EditNET.Helpers
{
    public static class ObjectExtensions
    {
        public static T SerializedCopy<T>(this T original)
        {
            string serialized = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<T>(serialized)!;
        }
    }
}