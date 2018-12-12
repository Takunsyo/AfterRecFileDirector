using Newtonsoft.Json;

namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    /// EPGStation API
    /// 'server addr'/recorded/{id}
    /// Object"ReserveProgram"
    /// </summary>
    public class Program : EPGDefault
    {
        public long id { get; set; }
        public long startAt { get; set; }
        public long endAt { get; set; }
        public bool isFree { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string extended { get; set; }
        public int genre1 { get; set; }
        public int genre2 { get; set; }
        public MirakurunWarpper.ChannelType channelType { get; set; }
        public string videoType  { get; set; }
        public string videoResolution  { get; set; }
        public int videoStreamContent  { get; set; }
        public int videoComponentType  { get; set; }
        public int audioSamplingRate  { get; set; }
        public int audioComponentType  { get; set; }
        public bool recording  { get; set; }
        public bool protection  { get; set; }
        public long filesize  { get; set; }
        public int errorCnt  { get; set; }
        public int dropCnt  { get; set; }
        public int scramblingCnt  { get; set; }
        public bool hasThumbnail  { get; set; }
        public bool original  { get; set; }
        public string filename  { get; set; }
        //public dynamic encoded { get; set; } //not needed
        //public dynamic encoding { get; set; }//not needed
        public bool overlap { get; set; }
        public long channelId { get; set; }
        public int eventId { get; set; }
        public int serviceId { get; set; }
        public int networkId { get; set; }
        public string channel { get; set; }
        [JsonIgnore]
        public bool IsOnGoing
        {
            get
            {
                var tnow = RVMCore.MirakurunWarpper.MirakurunService.GetUNIXTimeStamp();
                return (this.startAt <= tnow) && (this.endAt > tnow);
            }
        }
        [JsonIgnore]
        public ProgramGenre Genre
        {
            get
            {
                return (ProgramGenre)(1 << this.genre1);
            }
        }
        [JsonIgnore]
        public string GenreString
        {
            get
            {
                return ((ProgramGenre)(1 << this.genre1)).ToRecString();
            }
        }
        [JsonIgnore]
        public string SubGenre
        {
            get
            {
                switch (this.Genre)
                {
                    case ProgramGenre.Anime:
                        return ((AnimeGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Documantry:
                        return ((DocumantryGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Drama:
                        return ((DramaGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Education:
                        return ((EducationGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Infomation:
                        return ((EducationGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Live:
                        return ((LiveGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Movie:
                        return ((MovieGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Music:
                        return ((MusicGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.News:
                        return ((NewsGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Sports:
                        return ((SportsGenre)(1 << this.genre2)).ToString();
                    case ProgramGenre.Variety:
                        return ((VarietyGenre)(1 << this.genre2)).ToString();
                    default:
                        return "";
                }
            }
        }
        public byte[] Serialize()
        {
            var jStr = JsonConvert.SerializeObject(this,
                Formatting.None,
                new JsonSerializerSettings { //Ignore null properties to save space.
                    NullValueHandling = NullValueHandling.Ignore
                });
            return System.Text.Encoding.UTF8.GetBytes(jStr);
        }
        public static Program Deserialize(byte[] data)
        {
            var jStr = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<Program>(jStr);
        }
    }
}
