using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{
    public class @Event
    {
        public ResourceType resource { get; set; }
        public EventType type { get; set; }
        /// <summary>
        /// this object's type depens on <see cref="resource"/>, use <see cref="GetType"/> to findout true type,
        /// or use enum <see cref="ResourceType"/> to directcast target types.
        /// <para>Can be either <see cref="Program"/>, <see cref="Service"/> or <see cref="Tuner"/></para>
        /// </summary>
        public dynamic Data {
            get
            {
                if(_Data is null)
                {
                    string md = raw.ToString();
                    switch (this.resource)
                    {
                        case ResourceType.program:
                            this._Data = JsonConvert.DeserializeObject<Program>(md);
                            break;
                        case ResourceType.service:
                            this._Data = JsonConvert.DeserializeObject<Service>(md);
                            break;
                        case ResourceType.tuner:
                            this._Data = JsonConvert.DeserializeObject<Tuner>(md);
                            break;
                    }
                    raw = null;
                }
                return _Data;
            }
        }
        private dynamic _Data = null;
        [JsonProperty("data")]
        private object raw { get; set; }
        public long time { get; set; }
    }

    public enum EventType {
        create,
        update,
        redefine
    }

    public enum ResourceType
    {
        program,
        service,
        tuner
    }
}
