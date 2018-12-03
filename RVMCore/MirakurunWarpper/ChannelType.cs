using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper
{
    /// <summary>
    /// The enum dedicate to signal types.
    /// </summary>
    public enum ChannelType {
        /// <summary>
        /// Ground radio signals.
        /// </summary>
        GR,
        /// <summary>
        /// Broadcasting satellites signals. (110°)
        /// </summary>
        BS,
        /// <summary>
        /// Communication satellites signals. (110°)
        /// </summary>
        CS,
        /// <summary>
        /// Sky perfect specific broadcasting signals. (mostly in 128° broadcasting satellites)
        /// </summary>
        SKY
    }
}
