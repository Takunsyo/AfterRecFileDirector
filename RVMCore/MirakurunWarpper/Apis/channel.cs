
namespace RVMCore.MirakurunWarpper.Apis
{
    public class Channel
    {
        public string type { get; set; }
        public string channel { get; set; }
        public string name { get; set; }
        public string satelite { get; set; }
        public int space { get; set; }
        public Service[] services { get; set; }
    }
}
