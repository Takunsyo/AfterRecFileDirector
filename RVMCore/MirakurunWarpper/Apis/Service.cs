namespace RVMCore.MirakurunWarpper.Apis
{
    public class Service
    {
        public long id { get; set; }
        public int serviceId { get; set; }
        public int networkId { get; set; }
        public string name { get; set; }
        public int logoId { get; set; }
        public bool hasLogoData { get; set; }
        public int remoteControlKeyId { get; set; }
        public Channel channel { get; set; }
    }
}