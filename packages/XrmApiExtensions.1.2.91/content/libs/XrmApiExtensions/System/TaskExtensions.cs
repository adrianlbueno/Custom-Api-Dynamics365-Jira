using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs a Task Synchronous while setting "ConfigureAwait" to "false" as default
        /// (to prevent blocking in asp.net and other technologies where the thread handling the processing is important)
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="task"></param>
        /// <param name="ConfigureAwait"></param>
        /// <returns></returns>
        public static TReturn RunSynchronous<TReturn>(this Task<TReturn> task, Boolean ConfigureAwait = false)
        {
            task.ConfigureAwait(ConfigureAwait);
            task.Wait();
            return task.Result;
        }
    }
}
