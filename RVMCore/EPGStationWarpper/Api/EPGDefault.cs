namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    /// EPGStation api's basic response normally represent of error during process.
    /// </summary>
    public class EPGDefault
    {
        public int code { get; set; }

        public string message { get; set; }

        public string errors { get; set; }
                    
    }
}
