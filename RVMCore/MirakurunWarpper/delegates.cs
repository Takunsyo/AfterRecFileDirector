using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVMCore.MirakurunWarpper.Apis;

namespace RVMCore.MirakurunWarpper
{
    /// <summary>
    /// Handler to handle <see cref="MirakurunService.EventRecived"/> event.
    /// </summary>
    /// <param name="sender">The object who raised this event.</param>
    /// <param name="events">A <see cref="Event"/> object to describe dedicate informations of this event.</param>
    public delegate void EventRecivedHandler(object sender, Event events);

    /// <summary>
    /// Handler to handle <see cref="MirakurunService.LogRecived"/> event.
    /// </summary>
    /// <param name="sender">The object who raised this event.</param>
    /// <param name="log">A line of <see cref="string"/> in the log.</param>
    public delegate void LogRecivedHandler(object sender, string log);
}
