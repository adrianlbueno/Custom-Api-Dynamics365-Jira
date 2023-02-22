/**
 * ##############################################################################################################################
 *  
 *                !!!!!!!!!! THIS FILE IS DISTRIBUTED VIA NUGET! DO NOT CHANGE THIS FILE DIRECTLY!  !!!!!!!!!!
 *  
 *                !!!!!!!!!! EVERY CHANGE YOU MAKE WILL DEFINETLY BE LOST IF THE NUGET GETS UPDATED !!!!!!!!!!
 *                
 *            !!!!!!!!!! THE ONLY VALID WAY TO CHANGE THIS FILE IS CHANGING IT WITHIN THE XRMCOMMONLIB REPO !!!!!!!!!!
 *  
 * ##############################################################################################################################
 * */

using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    /// <summary>
    /// Provides helpful extension methods for the <see cref="EntityReference"/> class.
    /// </summary>
    public static class EntityReferenceExtensions
    {
        /// <summary>
        /// Functionpointer which will be called to log debug messages.
        /// By default used: <see cref="ConnectivOrgServiceExtensions.Debug"/>
        /// </summary>
        public static Action<string> Debug = ConnectivOrgServiceExtensions.Debug;

        /// <summary>
        /// Functionpointer which will be called to log info messages.
        /// By default used: <see cref="ConnectivOrgServiceExtensions.Info"/>
        /// </summary>
        public static Action<string> Info = ConnectivOrgServiceExtensions.Info;

        /// <summary>
        /// Functionpointer which will be called to log warning messages.
        /// By default used: <see cref="ConnectivOrgServiceExtensions.Warning"/>
        /// </summary>
        public static Action<string> Warning = ConnectivOrgServiceExtensions.Warning;

        /// <summary>
        /// Functionpointer which will be called to log error messages.
        /// By default used: <see cref="ConnectivOrgServiceExtensions.Error"/>
        /// </summary>
        public static Action<string> Error = ConnectivOrgServiceExtensions.Error;

        /// <summary>
        /// Functionpointer which will be called to send heart beats.
        /// By default used: <see cref="ConnectivOrgServiceExtensions.HeartBeat"/>
        /// </summary>
        public static Action HeartBeat = ConnectivOrgServiceExtensions.HeartBeat;

        /// <summary>
        /// Creates a new <see cref="Entity"/> instance and sets the id and logicalname.
        /// </summary>
        /// <param name="reference">An entityreference instance that contains the properties to set in the newly created record.</param>
        /// <returns>The newly created entity instance having the ID and logicalname property set.</returns>
        public static Entity ToEntity(this EntityReference reference)
        {
            return new Entity(reference.LogicalName, reference.Id);
        }

        public static TEntity ToEntity<TEntity>(this EntityReference reference) where TEntity : Entity, new()
        {
            return new TEntity().Set(v => v.Id = reference.Id);
        }

        #region Refresh

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/>.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this EntityReference entity, ColumnSet cols)
        {
            return entity.Refresh(cols, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this EntityReference entity, ColumnSet cols, IOrganizationService service)
        {
            return service.Refresh(entity, cols);
        }

        #endregion

        #region ToActivityParty

        /// <summary>
        /// Creates a new Entity instance of type activityparty and adds the <paramref name="references"/> as the partyid. Null values and empty ids will be ignored.
        /// You can set the returned array as "from" or "to" of an email.
        /// </summary>
        /// <param name="references">References to convert to activityparties. Null values and empty ids will be ignored.</param>
        /// <returns>List of activityparties each representing one of the <paramref name="references"/>.</returns>
        public static Entity[] ToActivityParty(this IEnumerable<EntityReference> references)
        {
            return references.Where(x => x != null && x.Id != Guid.Empty).Select(x => x.ToActivityParty()).ToArray();
        }

        /// <summary>
        /// Creates a new Entity instance of type activityparty and adds the <paramref name="reference"/> as the partyid.
        /// Put the returned instance into an Array and you can set it as "from" or "to" of an email.
        /// </summary>
        /// <param name="reference">Reference to convert to an activityparty.</param>
        /// <returns>New entity instance having the <paramref name="reference"/> as partyid filled.</returns>
        public static Entity ToActivityParty(this EntityReference reference)
        {
            return new Entity("activityparty") { Attributes = { { "partyid", reference } } };
        }
        
        #endregion
    }
}
