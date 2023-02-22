using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon
{
    public static class ExecuteMultipleExtensions
    {
        /// <summary>
        /// a list of possible operations
        /// </summary>
        public enum ExecuteMultipleOperation { unknown, delete, create, update, setstate }

        /// <summary>
        /// Gets the underlying operation of the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The type of the executed ExecuteMultipleOperation</returns>
        public static ExecuteMultipleOperation GetOperation(this OrganizationRequest request)
        {
            if (request is DeleteRequest)
                return ExecuteMultipleOperation.delete;
            else if (request is CreateRequest)
                return ExecuteMultipleOperation.create;
            else if (request is UpdateRequest)
                return ExecuteMultipleOperation.update;
            else if (request is SetStateRequest)
                return ExecuteMultipleOperation.setstate;
            else
                return ExecuteMultipleOperation.unknown;
        }

        /// <summary>
        /// extracts the target (entity) of the operation from the request
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Entity ToEntity(this ExecuteMultipleResponseItem instance, OrganizationRequest request)
        {
            switch (request.GetOperation())
            {
                case ExecuteMultipleOperation.delete:
                    return (request as DeleteRequest).Target.ToEntity();
                case ExecuteMultipleOperation.create:

                    Entity retVal = (request as CreateRequest).Target;
                    retVal.Id = (instance.Response as CreateResponse ?? new CreateResponse()).id;
                    return retVal;
                case ExecuteMultipleOperation.update:
                    return (request as UpdateRequest).Target;
                case ExecuteMultipleOperation.setstate:
                    return (request as SetStateRequest).EntityMoniker.ToEntity();
                default:
                    return null;
            }
        }

        /// <summary>
        /// extracts the target (entity) of the operation from a list of requests containing the provided specific request
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        public static Entity ToEntity(this ExecuteMultipleResponseItem instance, IEnumerable<OrganizationRequest> requests)
        {
            return instance.ToEntity(requests.ElementAt(instance.RequestIndex));
        }
    }
}
