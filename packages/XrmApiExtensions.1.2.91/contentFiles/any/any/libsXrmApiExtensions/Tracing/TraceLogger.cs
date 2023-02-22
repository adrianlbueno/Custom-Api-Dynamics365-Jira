using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon.Core.Tracing
{
    public enum TraceLevel
    {
        Trace = 0,
        Verbose = 1,
        Debug = 2,
        Info = 3,
        Error = 4,
        Fatal = 5
    }

    public class TraceLogger : ITracingService
    {
        public List<String> TraceMessages = new List<string>();

        public StringBuilder LogText { get; set; } = new StringBuilder();

        public TraceLevel LogThreshold { get; set; } = TraceLevel.Debug;

        private ITracingService traceService = null;

        public TraceLogger(ITracingService traceService, TraceLevel threshold = TraceLevel.Debug)
        {
            this.traceService = traceService;
            LogThreshold = threshold;
        }

        public void Trace(String message, TraceLevel? level = null)
        {
            String renderedMessage = level != null ? $"[{level.ToString().ToUpper()}] - {message}" : message;

            TraceMessages.Add(renderedMessage);

            LogText?.AppendLine(renderedMessage);

            if (level == null || level >= LogThreshold)
            {
                traceService?.Trace(renderedMessage);
            }
        }

        public void Trace(string message, params object[] args)
        {
            Trace(args.Length > 0 ? String.Format(message, args) : message );
        }
    }
}
