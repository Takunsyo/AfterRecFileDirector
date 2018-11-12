using System.Collections.Generic;

namespace RVMCore.MirakurunWarpper.Apis
{
    /// <summary>
    /// Response Class of "/programs"
    /// </summary>
    public class Program
    {
        public long id { get; set; }
        public int eventId { get; set; }
        public int serviceId { get; set; }
        public int networkId { get; set; }
        public long startAt { get; set; }
        public int duration { get; set; }
        public bool isFree { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Genre[] genres { get; set; }
        public Video video { get; set; }
        public Audio audio { get; set; }
        public Dictionary<string, string> extended { get; set; }
        public Relateditem[] relatedItems { get; set; }
        public Series series { get; set; }
    }

    public class Video
    {
        public string type { get; set; }
        public string resolution { get; set; }
        public int streamContent { get; set; }
        public int componentType { get; set; }
    }

    public class Audio
    {
        public int samplingRate { get; set; }
        public int componentType { get; set; }
    }
    
    public class Series
    {
        public int id { get; set; }
        public int repeat { get; set; }
        public int pattern { get; set; }
        public int expiresAt { get; set; }
        public int episode { get; set; }
        public int lastEpisode { get; set; }
        public string name { get; set; }
    }

    public class Genre
    {
        public int lv1 { get; set; }
        public int lv2 { get; set; }
        public int un1 { get; set; }
        public int un2 { get; set; }
    }

    public class Relateditem
    {
        public int networkId { get; set; }
        public int serviceId { get; set; }
        public int eventId { get; set; }
    }
}
