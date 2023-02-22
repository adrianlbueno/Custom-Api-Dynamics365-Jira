using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon.Core
{
    public class TracingService : ILogTracingService
    {
        private Action<String> PrintAction;
        private Dictionary<LogLevel, Action<String>> LogActions;

        public Func<LogLevel, String, String> MessageFormater { get; set; } = (level, message) => $"{DateTime.Now} : [{level.ToString().ToUpper()}] : {message}";

        public TracingService(Action<String> printAction)
        {
            PrintAction = printAction;
        }

        public TracingService(Dictionary<LogLevel, Action<String>> traceActions)
        {
            LogActions = traceActions;
        }

        public void Trace(string format, params object[] args)
        {
            PrintAction?.Invoke(String.Format(format, args));
        }
        protected void Print(LogLevel level, String message)
        {
            if (LogActions.ContainsKey(level))
            {
                LogActions[level]?.Invoke(MessageFormater?.Invoke(level, message) ?? message);
            }
            else
            {
                PrintAction?.Invoke(MessageFormater?.Invoke(level, message) ?? message);
            }
        }

        public void Debug(string message)
        {
            Print(LogLevel.Debug, message);
        }

        public void Error(string message)
        {
            Print(LogLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Print(LogLevel.Fatal, message);
        }

        public void Info(string message)
        {
            Print(LogLevel.Info, message);
        }

        public void Verbose(string message)
        {
            Print(LogLevel.Verbose, message);
        }

        public void Warning(string message)
        {
            Print(LogLevel.Warning, message);
        }

        public void Trace(string message)
        {
            Trace(message, new object[0]);
        }
    }
}
