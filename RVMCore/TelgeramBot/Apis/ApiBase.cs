using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis
{
    /// <summary>
    /// Base api calss.
    /// </summary>
    /// <typeparam name="T">Target object type under namespace <see cref="RVMCore.TelgeramBot.Apis.Objects"/></typeparam>
    public class ApiBase<T>
    {
        public bool ok { get; set; }

        public T result { get; set; }
    }
}
