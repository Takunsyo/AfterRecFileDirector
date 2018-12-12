namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    ///  EPGStation API
    /// 'server addr'/recorded/{id}
    /// </summary>
    public class EPGchannel : EPGDefault
    {
        public long id { get; set; }
        public int serviceId { get; set; }
        public int networkId { get; set; }
        public string name { get; set; }
        public int remoteControlKeyId { get; set; }
        public bool hasLogoData { get; set; }
        public string channelType { get; set; }
        public string channel { get; set; }
        public int type { get; set; }
    }
}
