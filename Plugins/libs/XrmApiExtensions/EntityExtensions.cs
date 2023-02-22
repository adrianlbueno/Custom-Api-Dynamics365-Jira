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


using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    /// <summary>
    /// Provides helpful extension methods for the <see cref="Entity"/> class.
    /// </summary>
    public static class EntityExtensions
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

        #region Create

        /// <summary>
        /// Creates a record and directly adds the ID to the <paramref name="entity"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">An entity instance that contains the properties to set in the newly created record.</param>
        /// <returns>The same entity instance having the ID property set.</returns>
        public static Entity Create(this Entity entity)
        {
            return entity.Create(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a record and directly adds the ID to the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">An entity instance that contains the properties to set in the newly created record.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same entity instance having the ID property set.</returns>
        public static Entity Create(this Entity entity, IOrganizationService service)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            entity.Id = service.Create(entity);
            return entity;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing record. Before updating the record in the CRM, the <paramref name="key"/> and <paramref name="value"/>
        /// will be added to the <paramref name="entity"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <param name="key">Attribute key to add to the <paramref name="entity"/>'s keys.</param>
        /// <param name="value">Attribute value to add to the <paramref name="entity"/>'s values.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity, string key, object value)
        {
            return entity.Update(key, value, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates an existing record. Before updating the record in the CRM, the <paramref name="key"/> and <paramref name="value"/>
        /// will be added to the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <param name="key">Attribute key to add to the <paramref name="entity"/>'s keys.</param>
        /// <param name="value">Attribute value to add to the <paramref name="entity"/>'s values.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity, string key, object value, IOrganizationService service)
        {
            return entity.Update(new AttributeCollection() { { key, value } }, service);
        }

        /// <summary>
        /// Updates an existing record. Before updating the record in the CRM, the <paramref name="attributes"/>
        /// will be added to the <paramref name="entity"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <param name="attributes">List of attributes to add before the update process.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity, DataCollection<string, object> attributes)
        {
            return entity.Update(attributes, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates an existing record. Before updating the record in the CRM, the <paramref name="attributes"/>
        /// will be added to the <paramref name="entity"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <param name="attributes">List of attributes to add before the update process.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity, DataCollection<string, object> attributes, IOrganizationService service)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            foreach (var attribute in attributes)
            {
                entity.Attributes[attribute.Key] = attribute.Value;
            }

            return entity.Update(service);
        }

        /// <summary>
        /// Updates an existing record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity)
        {
            return entity.Update(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates an existing record.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Update(this Entity entity, IOrganizationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            service.Update(entity);
            return entity;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record to delete.</param>
        public static void Delete(this Entity entity)
        {
            entity.Delete(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record to delete.</param>
        /// <param name="service">CRM Service instance.</param>
        public static void Delete(this Entity entity, IOrganizationService service)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            service.Delete(entity);
        }

        #endregion

        #region Activate

        /// <summary>
        /// Activates a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record to activate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Activate(this Entity entity, int status = -1)
        {
            return entity.Activate(ConnectivOrgServiceExtensions.OrganizationService, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record to activate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Activate(this Entity entity, OptionSetValue status)
        {
            return entity.Activate(ConnectivOrgServiceExtensions.OrganizationService, status);
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="entity">The record to activate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Activate(this Entity entity, IOrganizationService service, int status = -1)
        {
            return entity.Activate(service, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="entity">The record to activate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Activate(this Entity entity, IOrganizationService service, OptionSetValue status)
        {
            service.Activate(entity, status);
            return entity;
        }

        #endregion

        #region Deactivate

        /// <summary>
        /// Deactivates a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record to deactivate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Deactivate(this Entity entity, int status = -1)
        {
            return entity.Deactivate(ConnectivOrgServiceExtensions.OrganizationService, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates a record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record to deactivate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Deactivate(this Entity entity, OptionSetValue status)
        {
            return entity.Deactivate(ConnectivOrgServiceExtensions.OrganizationService, status);
        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="entity">The record to deactivate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Deactivate(this Entity entity, IOrganizationService service, int status = -1)
        {
            return entity.Deactivate(service, new OptionSetValue(status));

        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="entity">The record to deactivate.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Deactivate(this Entity entity, IOrganizationService service, OptionSetValue status)
        {
            service.Deactivate(entity, status);
            return entity;
        }

        #endregion

        #region SetState

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity SetState(this Entity entity, int state, int status = -1)
        {
            return entity.SetState(new OptionSetValue(status), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="entity">The record which status shall be set.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity SetState(this Entity entity, IOrganizationService service, int state, int status = -1)
        {
            return entity.SetState(service, new OptionSetValue(status), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity SetState(this Entity entity, OptionSetValue state, OptionSetValue status)
        {
            return entity.SetState(ConnectivOrgServiceExtensions.OrganizationService, status, status);
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="entity">The record which status shall be set.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity SetState(this Entity entity, IOrganizationService service, OptionSetValue state, OptionSetValue status)
        {
            service.SetState(entity, state, status);
            return entity;
        }

        #endregion

        #region Associate

        /// <summary>
        /// Creates a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate(this Entity entity, Relationship relationship, EntityCollection relatedEntities)
        {
            return entity.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate(this Entity entity, Relationship relationship, EntityCollection relatedEntities, IOrganizationService service)
        {
            return entity.Associate(relationship, relatedEntities.Entities, service);
        }

        /// <summary>
        /// Creates a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate<T>(this Entity entity, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity
        {
            return entity.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate<T>(this Entity entity, Relationship relationship, IEnumerable<T> relatedEntities, IOrganizationService service)
            where T : Entity
        {
            return entity.Associate(relationship, relatedEntities?.Select(x => x?.ToEntityReference()), service);
        }

        /// <summary>
        /// Creates a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate(this Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            return entity.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="entity">Record will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Associate(this Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities, IOrganizationService service)
        {
            service.Associate(entity, relationship, relatedEntities);
            return entity;
        }

        #endregion

        #region Disassociate

        /// <summary>
        /// Deletes a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate(this Entity entity, Relationship relationship, EntityCollection relatedEntities)
        {
            return entity.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate(this Entity entity, Relationship relationship, EntityCollection relatedEntities, IOrganizationService service)
        {
            return entity.Disassociate(relationship, relatedEntities.Entities, service);
        }

        /// <summary>
        /// Deletes a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate<T>(this Entity entity, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity
        {
            return entity.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate<T>(this Entity entity, Relationship relationship, IEnumerable<T> relatedEntities, IOrganizationService service)
            where T : Entity
        {
            return entity.Disassociate(relationship, relatedEntities?.Select(x => x?.ToEntityReference()), service);
        }

        /// <summary>
        /// Deletes a link between records.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate(this Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            return entity.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="entity">Record will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Disassociate(this Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities, IOrganizationService service)
        {
            service.Disassociate(entity, relationship, relatedEntities);
            return entity;
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Updates all attributes already set in the record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/>.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this Entity entity)
        {
            return entity.Refresh(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/>.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this Entity entity, ColumnSet cols)
        {
            return entity.Refresh(cols, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Updates all attributes already set in the record.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this Entity entity, IOrganizationService service)
        {
            return service.Refresh(entity);
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same entity instance.</returns>
        public static Entity Refresh(this Entity entity, ColumnSet cols, IOrganizationService service)
        {
            return service.Refresh(entity, cols);
        }

        #endregion

        #region TryGetValue

        public static T GetAliasedValue<T>(this Entity entity, string attributeName)
        {
            object retVal = entity?.GetAttributeValue<AliasedValue>(attributeName)?.Value;
            return retVal is T ? (T)retVal : default;
        }

        /// <summary>
        /// Converts the value for the <paramref name="attributeName"/> to the requested type <typeparamref name="T"/>. 
        /// Returns false if the <paramref name="entity"/> does not contain the requested <paramref name="attributeName"/> or the conversion failed.
        /// </summary>
        /// <typeparam name="T">Type to which the attribute's value will be converted.</typeparam>
        /// <param name="entity">Entity record containing the attribute.</param>
        /// <param name="attributeName">Internal logical name of the attribute.</param>
        /// <param name="value">Will contain the converted result, or default(<typeparamref name="T"/>) if something went wrong.</param>
        /// <returns>True if attribute was found and value could be converted, else false.</returns>
        public static bool TryGetValue<T>(this Entity entity, string attributeName, out T value)
        {
            return entity.TryGetValue<T>(attributeName, default(T), out value);
        }

        /// <summary>
        /// Converts the value for the <paramref name="attributeName"/> to the requested type <typeparamref name="T"/>. 
        /// Returns false if the <paramref name="entity"/> does not contain the requested <paramref name="attributeName"/> or the conversion failed.
        /// </summary>
        /// <typeparam name="T">Type to which the attribute's value will be converted.</typeparam>
        /// <param name="entity">Entity record containing the attribute.</param>
        /// <param name="attributeName">Internal logical name of the attribute.</param>
        /// <param name="defaultValue">Will be set as out value if conversion was not successfull.</param>
        /// <param name="value">Will contain the converted result, or <paramref name="defaultValue"/> if something went wrong.</param>
        /// <returns>True if attribute was found and value could be converted, else false.</returns>
        public static bool TryGetValue<T>(this Entity entity, string attributeName, T defaultValue, out T value)
        {
            value = entity.GetAttributeValue<T>(attributeName);
            if (value == null) value = defaultValue;

            return entity.Attributes.ContainsKey(attributeName);
        }

        /// <summary>
        /// Tries getting the value for the <paramref name="attributeName"/> from the <paramref name="entity"/>. If the
        /// <paramref name="entity"/> does not contain the attribute, the <paramref name="image"/> will be searched.
        /// The found value for the <paramref name="attributeName"/> will be converted to the requested type <typeparamref name="T"/>.
        /// Returns false if the <paramref name="entity"/> does not contain the requested <paramref name="attributeName"/> or the conversion failed.
        /// </summary>
        /// <typeparam name="T">Type to which the attribute's value will be converted.</typeparam>
        /// <param name="entity">Entity record containing the attribute.</param>
        /// <param name="attributeName">Internal logical name of the attribute.</param>
        /// <param name="image">Backup entity record which will be searched if the <paramref name="entity"/> does not contain the attribute.</param>
        /// <param name="value">Will contain the converted result, or default(<typeparamref name="T"/>) if something went wrong.</param>
        /// <returns>True if attribute was found in either of the two entity instances and value could be converted, else false.</returns>
        public static bool TryGetValue<T>(this Entity entity, string attributeName, Entity image, out T value)
        {
            return entity.TryGetValue<T>(attributeName, image, default(T), out value);
        }

        /// <summary>
        /// Tries getting the value for the <paramref name="attributeName"/> from the <paramref name="entity"/>. If the
        /// <paramref name="entity"/> does not contain the attribute, the <paramref name="image"/> will be searched.
        /// The found value for the <paramref name="attributeName"/> will be converted to the requested type <typeparamref name="T"/>.
        /// Returns false if the <paramref name="entity"/> does not contain the requested <paramref name="attributeName"/> or the conversion failed.
        /// </summary>
        /// <typeparam name="T">Type to which the attribute's value will be converted.</typeparam>
        /// <param name="entity">Entity record containing the attribute.</param>
        /// <param name="attributeName">Internal logical name of the attribute.</param>
        /// <param name="image">Backup entity record which will be searched if the <paramref name="entity"/> does not contain the attribute.</param>
        /// <param name="defaultValue">Will be set as out value if conversion was not successfull.</param>
        /// <param name="value">Will contain the converted result, or <paramref name="defaultValue"/> if something went wrong.</param>
        /// <returns>True if attribute was found in either of the two entity instances and value could be converted, else false.</returns>
        public static bool TryGetValue<T>(this Entity entity, string attributeName, Entity image, T defaultValue, out T value)
        {
            return entity.TryGetValue(attributeName, defaultValue, out value) || image.TryGetValue(attributeName, defaultValue, out value);
        }

        #endregion

        #region ToDetailedString
        public static string ToDetailedString(this Entity entity, bool printEntityCollectionDetailed = false)
        {
            try
            {
                List<string> attributesAsStringToPrint = new List<string>();
                int maxLength = entity.Attributes.Max(x => x.Key.Length);

                foreach (KeyValuePair<string, object> attribute in entity.Attributes)
                {
                    string key = attribute.Key;
                    string value = $"[{attribute.Value?.GetType().Name}] ";

                    switch (attribute.Value)
                    {
                        case bool b:
                        case DateTime dt:
                        case double d:
                        case Guid g:
                        case int i:
                        case long l:
                        case string s:
                            value += attribute.Value.ToString().Replace("\r\n", $"{Environment.NewLine}{"".PadLeft(maxLength + 5)}");
                            break;
                        case BooleanManagedProperty bmp:
                            value += bmp.Value.ToString();
                            break;
                        case decimal d:
                            value += d.ToString("{0:n}");
                            break;
                        case EntityCollection ec:

                            IEnumerable<string> ecString;

                            if (printEntityCollectionDetailed)
                            {
                                ecString = ec.Entities.Select(x => x.ToDetailedString(true).Replace("\r\n", $"{Environment.NewLine}\t"));
                            }
                            else
                            {
                                ecString = ec.Entities.Select(x => $"{{{x.LogicalName}: {x.Id}}}");
                            }

                            value += $"{Environment.NewLine}{{{Environment.NewLine}{"".PadLeft(maxLength + 5)}" + string.Join($",{Environment.NewLine}{"".PadLeft(maxLength + 5)}", ecString) + $"{Environment.NewLine}\r}}";

                            break;
                        case EntityReference er:
                            value += $"{{{er.LogicalName}: {er.Id}}}";
                            break;
                        case Money mn:
                            value += mn.Value.ToString("C");
                            break;
                        case OptionSetValue osv:
                            value += osv.Value;
                            break;
                        case AliasedValue asv:
                            value += asv?.Value.ToDetailedString();
                            break;
                        default:
                            value += attribute.Value?.ToString() ?? "null";
                            break;
                    }

                    attributesAsStringToPrint.Add($"{{{key.PadRight(maxLength)}:\t{value}}}");
                }

                attributesAsStringToPrint.Sort();
                return $"{Environment.NewLine}[{entity.LogicalName}]{Environment.NewLine}{string.Join($", {Environment.NewLine}", attributesAsStringToPrint)}";
            }
            catch (Exception e)
            {
                return $"An error occured while parsing the attributes to string: {e.Message}";
            }
        }
        #endregion

        #region ToActivityParty
        /// <summary>
        /// Creates a new Entity instance of type activityparty and adds the <paramref name="reference"/> as the partyid.
        /// Put the returned instance into an Array and you can set it as "from" or "to" of an email.
        /// </summary>
        /// <param name="reference">Reference to convert to an activityparty.</param>
        /// <returns>New entity instance having the <paramref name="reference"/> as partyid filled.</returns>
        public static Entity ToActivityParty(this Entity references)
        {
            return new Entity("activityparty") { Attributes = { { "partyid", references.ToEntityReference() } } };
        }

        /// <summary>
        /// Creates a new Entity instance of type activityparty and adds the <paramref name="references"/> as the partyid. Null values and empty ids will be ignored.
        /// You can set the returned array as "from" or "to" of an email.
        /// </summary>
        /// <param name="references">References to convert to activityparties. Null values and empty ids will be ignored.</param>
        /// <returns>List of activityparties each representing one of the <paramref name="references"/>.</returns>
        public static Entity[] ToActivityParty(this IEnumerable<Entity> references)
        {
            return references.Where(x => x != null && x.Id != Guid.Empty).Select(x => x.ToActivityParty()).ToArray();
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Removes all References to the current primary key from the attribute collection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Entity RemoveId(this Entity entity)
        {
            var legacyId = entity.Id;
            entity.Id = Guid.Empty;
            return entity.RemoveAttribute(entity.Attributes.Where(v => (v.Key == $"{entity.LogicalName}id" || legacyId.Equals(v.Value))).Select(attr => attr.Key).Distinct().ToList());
        }
        /// <summary>
        /// Removes all provided attributes from the internal collection - no error is thrown if any attribute is not contained
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributesToRemove"></param>
        /// <returns></returns>
        public static Entity RemoveAttribute(this Entity entity, params string[] attributesToRemove)
        {
            return entity.RemoveAttribute(attributesToRemove.ToList());
        }

        /// <summary>
        /// Removes all provided attributes from the internal collection - no error is thrown if any attribute is not contained
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributesToRemove"></param>
        /// <returns></returns>
        public static Entity RemoveAttribute(this Entity entity, IEnumerable<string> attributesToRemove)
        {
            foreach (var attributeToRemove in attributesToRemove.Where(v => entity.Contains(v)))
            {
                entity.Attributes.Remove(attributeToRemove);
            }

            return entity;
        }

        public static TEntity MergeAttributes<TEntity>(this TEntity source, TEntity additionalValues) where TEntity : Entity
        {
            // throws "System.Security.VerificationException: Operation could destabilize the runtime" in plugins:
            //return source?.Set(v => additionalValues?.Attributes.Where(attr => !v.Contains(attr.Key)).Foreach(newAttr => v[newAttr.Key] = newAttr.Value)) ?? additionalValues;

            //works even in plugins:
            additionalValues?.Attributes.Where(attr => !source.Contains(attr.Key)).Foreach(newAttr => source[newAttr.Key] = newAttr.Value);
            return source;
        }
        #endregion

        #region Exists

        /// <summary>
        /// Checks if the <paramref name="entity"/>'s id exists. (Using <see cref="QueryExpression"/> to prevent exceptions.)
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">This record will be checked.</param>
        /// <returns>True if a record having the requested id exists, otherwise false.</returns>
        public static bool Exists(this Entity entity)
        {
            return entity.Exists(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Checks if the <paramref name="entity"/>'s id exists. (Using <see cref="QueryExpression"/> to prevent exceptions.)
        /// </summary>
        /// <param name="entity">This record will be checked.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>True if a record having the requested id exists, otherwise false.</returns>
        public static bool Exists(this Entity entity, IOrganizationService service)
        {
            return service.Exists(entity);
        }

        #endregion

        #region RetrieveMetadata

        /// <summary>
        /// Retrieves all metadata of the the <paramref name="entity"/>'s table.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">Record to get all metadata from.</param>
        /// <returns>All entity metadata information for the requested table.</returns>
        public static EntityMetadata RetrieveMetadata(this Entity entity)
        {
            return entity.RetrieveMetadata(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Retrieves all metadata of the the <paramref name="entity"/>'s table.
        /// </summary>
        /// <param name="entity">Record with logicalname to get the table's metadata from.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>All entity metadata information for the requested table.</returns>
        public static EntityMetadata RetrieveMetadata(this Entity entity, IOrganizationService service)
        {
            return service.RetrieveMetadata(entity);
        }

        #endregion
        public static string GetAttributeLogicalName(this Entity entity, [CallerMemberName] string membername = "")
        {
            String retVal = membername.ToLower();
            PropertyInfo property = entity.GetType().GetProperties().FirstOrDefault(v => v.Name.Equals(membername));
            if (property != null)
            {
                AttributeLogicalNameAttribute logicalName = property.GetCustomAttribute<AttributeLogicalNameAttribute>();
                if (logicalName != null)
                {
                    retVal = logicalName.LogicalName;
                }
            }
            return retVal;
        }

    }
}
