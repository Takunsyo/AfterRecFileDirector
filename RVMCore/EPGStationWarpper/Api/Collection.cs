using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    /// A univarsal collection type of EPGStation data object.
    /// <para>See API #Definition:Total</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Collection<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }
        /// <summary>
        /// Deserialize a Json object to <see cref="Collection{T}"/>
        /// </summary>
        /// <param name="jsonString">Json object body string</param>
        /// <param name="mainCollection">Target property name if nesscery
        /// <para>Default will be type <see cref="T"/>'s name plus a char
        /// 's' in lower case.</para></param>
        /// <returns></returns>
        public static Collection<T> DeserializObject(string jsonString,string mainCollection = null)
        {
            var result = new Collection<T>();
            mainCollection = mainCollection.IsNullOrEmptyOrWhiltSpace() ? 
                typeof(T).Name.ToLower() + "s" : mainCollection;
            JObject obj = (JObject)JsonConvert.DeserializeObject(jsonString);
            result.Items = obj.GetValue(mainCollection).ToObject<IEnumerable<T>>();
            result.Count = obj.GetValue("total").ToObject<int>();
            return result;
        }
    }
}
