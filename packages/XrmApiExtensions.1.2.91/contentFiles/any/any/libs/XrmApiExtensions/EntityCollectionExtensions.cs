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
    /// Provides helpful extension methods for the <see cref="EntityCollection"/> class.
    /// </summary>
    public static class EntityCollectionExtensions
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
        /// Creates every record contained in the <paramref name="collection"/>. The created id will
        /// directly be appended to the record in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of records to create.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Create(this EntityCollection collection)
        {
            return collection.Create(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates every record contained in the <paramref name="collection"/>. The created id will
        /// directly be appended to the record in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">List of records to create.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Create(this EntityCollection collection, IOrganizationService service)
        {
            service.Create(collection);
            return collection;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored. Before updating the record in the CRM, the <paramref name="key"/> and <paramref name="value"/>
        /// will be added each record.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <param name="key">Attribute key to add to the <paramref name="entity"/>'s keys.</param>
        /// <param name="value">Attribute value to add to the <paramref name="entity"/>'s values.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection, string key, object value)
        {
            return collection.Update(new AttributeCollection() { { key, value } }, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored. Before updating the record in the CRM, the <paramref name="key"/> and <paramref name="value"/>
        /// will be added each record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <param name="key">Attribute key to add to the <paramref name="entity"/>'s keys.</param>
        /// <param name="value">Attribute value to add to the <paramref name="entity"/>'s values.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection, string key, object value, IOrganizationService service)
        {
            return collection.Update(new AttributeCollection() { { key, value } }, service);
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored. Before updating the record in the CRM, the <paramref name="attributes"/>
        /// will be added each record.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <param name="attributes">List of attributes to add before the update process.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection, DataCollection<string, object> attributes)
        {
            return collection.Update(attributes, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored. Before updating the record in the CRM, the <paramref name="attributes"/>
        /// will be added each record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <param name="attributes">List of attributes to add before the update process.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection, DataCollection<string, object> attributes, IOrganizationService service)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var entity in collection.Entities)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(collection)}. Skipping.");
                    continue;
                }

                entity.Update(attributes, service);
            }

            return collection;
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection)
        {
            return collection.Update(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">List of records to update.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Update(this EntityCollection collection, IOrganizationService service)
        {
            service.Update(collection);
            return collection;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of records to delete.</param>
        public static void Delete(this EntityCollection collection)
        {
            collection.Delete(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">List of records to delete.</param>
        /// <param name="service">CRM Service instance.</param>
        public static void Delete(this EntityCollection collection, IOrganizationService service)
        {
            service.Delete(collection);
        }

        #endregion

        #region Activate

        /// <summary>
        /// Activates all records contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Activate(this EntityCollection collection, int status = -1)
        {
            return collection.Activate(ConnectivOrgServiceExtensions.OrganizationService, status);
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">The records which shall be activated.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Activate(this EntityCollection collection, IOrganizationService service, int status = -1)
        {
            service.Activate(collection, status);
            return collection;
        }

        #endregion

        #region Deactivate

        /// <summary>
        /// Deactivates all records contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Deactivate(this EntityCollection collection, int status = -1)
        {
            return collection.Deactivate(ConnectivOrgServiceExtensions.OrganizationService, status);
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">The records which shall be deactivated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Deactivate(this EntityCollection collection, IOrganizationService service, int status = -1)
        {
            service.Deactivate(collection, status);
            return collection;
        }

        #endregion

        #region SetState

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection SetState(this EntityCollection collection, int state, int status = -1)
        {
            return collection.SetState(ConnectivOrgServiceExtensions.OrganizationService, state, status);
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">The records which status shall be set.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection SetState(this EntityCollection collection, IOrganizationService service, int state, int status = -1)
        {
            service.SetState(collection, state, status);
            return collection;
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection SetState(this EntityCollection collection, OptionSetValue state, OptionSetValue status)
        {
            return collection.SetState(ConnectivOrgServiceExtensions.OrganizationService, state, status);
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="collection">The records which status shall be set.</param>
        /// <param name="service">CRM service instance.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection SetState(this EntityCollection collection, IOrganizationService service, OptionSetValue state, OptionSetValue status)
        {
            service.SetState(collection, state, status);
            return collection;
        }

        #endregion

        #region Associate

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate(this EntityCollection collection, Relationship relationship, EntityCollection relatedEntities)
        {
            return collection.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate(this EntityCollection collection, Relationship relationship, EntityCollection relatedEntities, IOrganizationService service)
        {
            service.Associate(collection, relationship, relatedEntities);
            return collection;
        }

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate<T>(this EntityCollection collection, Relationship relationship, IEnumerable<T> relatedEntities)
            where T: Entity, new()
        {
            return collection.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate<T>(this EntityCollection collection, Relationship relationship, IEnumerable<T> relatedEntities, IOrganizationService service)
            where T : Entity, new()
        {
            service.Associate(collection, relationship, relatedEntities);
            return collection;
        }

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate(this EntityCollection collection, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            return collection.Associate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Creates a link between records. Null references in the entities will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Associate(this EntityCollection collection, Relationship relationship, IEnumerable<EntityReference> relatedEntities, IOrganizationService service)
        {
            service.Associate(collection, relationship, relatedEntities);
            return collection;
        }

        #endregion

        #region Disassociate

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate(this EntityCollection collection, Relationship relationship, EntityCollection relatedEntities)
        {
            return collection.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate(this EntityCollection collection, Relationship relationship, EntityCollection relatedEntities, IOrganizationService service)
        {
            service.Disassociate(collection, relationship, relatedEntities);
            return collection;
        }

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate<T>(this EntityCollection collection, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity, new()
        {
            return collection.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate<T>(this EntityCollection collection, Relationship relationship, IEnumerable<T> relatedEntities, IOrganizationService service)
            where T : Entity, new()
        {
            service.Disassociate(collection, relationship, relatedEntities);
            return collection;
        }

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate(this EntityCollection collection, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            return collection.Disassociate(relationship, relatedEntities, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Deletes a link between records. Null references in the collection will be ignored.
        /// </summary>
        /// <param name="collection">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same EntityCollection instance.</returns>
        public static EntityCollection Disassociate(this EntityCollection collection, Relationship relationship, IEnumerable<EntityReference> relatedEntities, IOrganizationService service)
        {
            service.Disassociate(collection, relationship, relatedEntities);
            return collection;
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="collection"/>. Updates all attributes already set in the record.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same instance as in the param having all non-null columns.</returns>
        public static EntityCollection Refresh(this EntityCollection collection)
        {
            return collection.Refresh(ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="collection"/>. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="collection">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same instance as in the param having all non-null columns.</returns>
        public static EntityCollection Refresh(this EntityCollection collection, ColumnSet cols)
        {
            return collection.Refresh(cols, ConnectivOrgServiceExtensions.OrganizationService);
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="collection"/>. Updates all attributes already set in the record.
        /// </summary>
        /// <param name="collection">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same instance as in the param having all non-null columns.</returns>
        public static EntityCollection Refresh(this EntityCollection collection, IOrganizationService service)
        {
            return service.Refresh(collection);
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="collection"/>. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="collection">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <param name="service">CRM Service Instance.</param>
        /// <returns>The same instance as in the param having all non-null columns.</returns>
        public static EntityCollection Refresh(this EntityCollection collection, ColumnSet cols, IOrganizationService service)
        {
            return service.Refresh(collection, cols);
        }

        #endregion

        /// <summary>
        /// Builds a CSV file using the <paramref name="attributes"/> keys as header of the file. 
        /// Each line will be evaluated using the <paramref name="attributes"/> values and the <paramref name="recordsToParse"/>.
        /// Linebreaks and semi-colons will automatically be removed while parsing the value.
        /// Any non-successfull attempt of parsing a value will result in an empty value added to the csv.
        /// </summary>
        /// <param name="recordsToParse">List of all records to parse as csv file.</param>
        /// <param name="attributes">Key is HEADER for csv. Value will be evaluated for each record. Supports all simple and following special types: 
        /// OptionSetValue, EntityReference
        /// </param>
        /// <returns>A StringBuilder instance holding the generated csv file.</returns>
        public static StringBuilder GenerateCsv(this IEnumerable<Entity> recordsToParse, Dictionary<string, string> attributes)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine(String.Join(";", attributes.Keys));

            foreach (var item in recordsToParse.Where(x => x != null))
            {
                var singleLineAttributes = new List<string>();
                foreach (var attribute in attributes)
                {
                    string stringValue = string.Empty;

                    var value = item.Attributes.ContainsKey(attribute.Value) ? item.Attributes[attribute.Value] : string.Empty;

                    switch (value)
                    {
                        case OptionSetValue os:
                            stringValue = os.Value.ToString();
                            break;
                        case EntityReference er:
                            stringValue = er.Id.ToString();
                            break;
                        default:
                            stringValue = value.ToString();
                            break;
                        case null:
                            break;
                    }

                    stringValue = stringValue.Replace("\r", "").Replace("\n", "").Replace(";", "");

                    singleLineAttributes.Add(stringValue);
                }

                csvBuilder.AppendLine(string.Join(";", singleLineAttributes));
            }

            return csvBuilder;
        }
    }
}
