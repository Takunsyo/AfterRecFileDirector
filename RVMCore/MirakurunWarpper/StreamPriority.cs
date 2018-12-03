using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper
{
    /// <summary>
    /// The enum to dedicate priority of Mirakurun stream services.
    /// </summary>
    public enum StreamPriority
    {
        /// <summary>
        /// This is the same level with EPG gathering process.
        /// </summary>
        Low = -1,
        /// <summary>
        /// This is the default for Mirakurun.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Normaly use this one is good enough.
        /// </summary>
        Normal = 1,
        AboveNormal = 2,
        SlightlyHigh = 3,
        High =4,
        Higher = 5,
        /// <summary>
        /// Oh yeah, wanna fu*k everyone else up? Try this.
        /// </summary>
        Critical = int.MaxValue
    }
}
