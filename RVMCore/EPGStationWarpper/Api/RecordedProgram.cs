using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    /// EPGStation API
    /// 'server addr'/recorded/{id}
    /// </summary>
    [DataContract]
    public class RecordedProgram : EPGDefault
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public long channelId { get; set; }
        [DataMember]
        public string channelType { get; set; }
        [DataMember]
        public long startAt { get; set; }
        [DataMember]
        public long endAt { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string extended { get; set; }
        [DataMember]
        public int genre1 { get; set; }
        [DataMember]
        public int genre2 { get; set; }
        public ProgramGenre Genre
        {
            get
            {
                return (ProgramGenre)(1 << this.genre1);
            }
        }
        private string GenreString
        {
            get
            {
                return ((ProgramGenre)(1<<this.genre1)).ToRecString();
            }
        }
        [DataMember]
        public string videoType  { get; set; }
        [DataMember]
        public string videoResolution  { get; set; }
        [DataMember]
        public int videoStreamContent  { get; set; }
        [DataMember]
        public int videoComponentType  { get; set; }
        [DataMember]
        public int audioSamplingRate  { get; set; }
        [DataMember]
        public int audioComponentType  { get; set; }
        [DataMember]
        public bool recording  { get; set; }
        [DataMember]
        public bool protection  { get; set; }
        [DataMember]
        public long filesize  { get; set; }
        [DataMember]
        public int errorCnt  { get; set; }
        [DataMember]
        public int dropCnt  { get; set; }
        [DataMember]
        public int scramblingCnt  { get; set; }
        [DataMember]
        public bool hasThumbnail  { get; set; }
        [DataMember]
        public bool original  { get; set; }
        [DataMember]
        public string filename  { get; set; }
        public dynamic encoded { get; set; } //not needed
        public dynamic encoding { get; set; }//not needed

        public byte[] Serialize()
        {
            var jStr=JsonConvert.SerializeObject(this);
            return System.Text.Encoding.UTF8.GetBytes(jStr);
        }
        public static RecordedProgram Deserialize(byte[] data)
        {
            var jStr = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<RecordedProgram>(jStr);
        }
    }
}
