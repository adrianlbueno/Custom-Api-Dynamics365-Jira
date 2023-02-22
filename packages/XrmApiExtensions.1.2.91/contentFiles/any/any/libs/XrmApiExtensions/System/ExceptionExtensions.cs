using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Text;

namespace System
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts an OrganizationServiceFault to a String representation containing detailed information about the error occured
        /// </summary>
        /// <param name="fault"></param>
        /// <returns>a String representation containing detailed information about the error occured</returns>
        public static String ToDetailedString(this OrganizationServiceFault fault)
        {
            String retVal = "";

            if (null != fault)
            {
                retVal += "[" + fault.Timestamp + "] :: Code:" + fault.ErrorCode + " Message:" + fault.Message + Environment.NewLine + "Trace:" + fault.TraceText;
                if (null != fault.InnerFault)
                    retVal += Environment.NewLine + "InnerException:" + Environment.NewLine + fault.InnerFault.ToDetailedString();
            }

            return retVal;
        }

        /// <summary>
        /// Prints out a detailed string containing all needed information about the occured exception
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="depth">The depth to which inner exceptions should be printed.</param>
        /// <returns></returns>
        public static String ToDetailedString(this Exception instance, int depth = 0)
        {
            if (instance == null)
            {
                return "";
            }

            String space = String.Empty.PadLeft(depth);
            StringBuilder retVal = new StringBuilder();

            retVal.AppendLine($"{space}Message: " + instance.Message);
            retVal.AppendLine($"{space}Type: " + instance.GetType().ToString());
            retVal.AppendLine($"{space}StackTrace: " + instance.StackTrace);

            if (instance is FaultException<OrganizationServiceFault>)
            {
                retVal.AppendLine($"{space}TimeStamp: " + (instance as FaultException<OrganizationServiceFault>)?.Detail.Timestamp);
                retVal.AppendLine($"{space}TraceText: " + (instance as FaultException<OrganizationServiceFault>)?.Detail.TraceText);
                retVal.AppendLine($"{space}InnerFault: " + (instance as FaultException<OrganizationServiceFault>)?.Detail.InnerFault);
            }

            if (instance.InnerException != null)
            {
                retVal.AppendLine($"{space}InnerException: {instance.InnerException?.ToDetailedString(depth + 1)}");
            }

            return retVal.ToString();
        }
    }
}
