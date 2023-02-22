using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon.Core
{
    public enum LogLevel
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public interface ILogTracingService : ITracingService
    {
        void Trace(String message);
        void Verbose(String message);
        void Debug(String message);
        void Info(String message);
        void Warning(String message);
        void Error(String message);
        void Fatal(String message);
    }
}
