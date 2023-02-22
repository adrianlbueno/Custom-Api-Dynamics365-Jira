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

using Connectiv.XrmCommon.Core.EarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Organization;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    /// <summary>
    /// Provides helpful extension methods for the <see cref="IOrganizationService"/> interface.
    /// </summary>
    public static class BasicOrganizationServiceExtensions
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

        #region WhoAmI
        public static WhoAmIResponse WhoAmI(this IOrganizationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return (service?.Execute(new WhoAmIRequest()) as WhoAmIResponse) ?? null;
        }

        /// <summary>
        /// Retrieves the OrganizationInfo (containing all installed solutions) of the current Organization
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static OrganizationInfo GetOrganizationInfo(this IOrganizationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return (service?.Execute(new RetrieveOrganizationInfoRequest()) as RetrieveOrganizationInfoResponse)?.organizationInfo ?? null;
        }


        #endregion

        #region Create

        /// <summary>
        /// Creates every record contained in the <paramref name="collection"/>. The created ID will
        /// directly be appended to the record in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to create.</param>
        public static void Create(this IOrganizationService service, EntityCollection collection)
        {
            service.Create(collection?.Entities);
        }

        /// <summary>
        /// Creates every record contained in the <paramref name="collection"/>. The created ID will
        /// directly be appended to the record in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to create.</param>
        public static void Create<T>(this IOrganizationService service, IEnumerable<T> collection)
            where T : Entity, new()
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var record in collection)
            {
                if (record == null)
                {
                    Warning($"Found a Null reference in {nameof(collection)}. Skipping.");
                    continue;
                }

                record.Id = service.Create(record);
            }
        }

        public static T Create<T>(this IOrganizationService service, T record) where T : Entity
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            record.Id = service.Create(record);
            return record;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to update.</param>
        public static void Update(this IOrganizationService service, EntityCollection collection)
        {
            service.Update(collection?.Entities);
        }

        /// <summary>
        /// Updates every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to update.</param>
        public static void Update<T>(this IOrganizationService service, IEnumerable<T> collection)
            where T : Entity, new()
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var record in collection)
            {
                if (record == null)
                {
                    Warning($"Found a Null reference in {nameof(collection)}. Skipping.");
                    continue;
                }

                service.Update(record);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to delete.</param>
        public static void Delete(this IOrganizationService service, EntityCollection collection)
        {
            service.Delete(collection?.Entities);
        }

        /// <summary>
        /// Deletes every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to delete.</param>
        public static void Delete<T>(this IOrganizationService service, IEnumerable<T> collection)
            where T : Entity, new()
        {
            service.Delete(collection?.Select(x => x.ToEntityReference()));
        }

        /// <summary>
        /// Deletes every record contained in the <paramref name="collection"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="collection">List of records to delete.</param>
        public static void Delete(this IOrganizationService service, IEnumerable<EntityReference> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var record in collection)
            {
                if (record == null)
                {
                    Warning($"Found a Null reference in {nameof(collection)}. Skipping.");
                    continue;
                }

                service.Delete(record);
            }
        }

        /// <summary>
        /// Deletes the <paramref name="record"/>.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="record">Record to delete.</param>
        public static void Delete(this IOrganizationService service, Entity record)
        {
            service.Delete(record?.ToEntityReference());
        }

        /// <summary>
        /// Deletes the <paramref name="record"/>.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="record">Record to delete.</param>
        public static void Delete(this IOrganizationService service, EntityReference record)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            service.Delete(record.LogicalName, record.Id);
        }

        #endregion

        #region Associate

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, EntityCollection entities, Relationship relationship, EntityCollection relatedEntities)
        {
            service.Associate(entities?.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, EntityCollection relatedEntities)
            where T : Entity, new()
        {
            service.Associate(entities, relationship, relatedEntities?.Entities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate<T>(this IOrganizationService service, EntityCollection entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity, new()
        {
            service.Associate(entities?.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity, new()
        {
            service.Associate(entities, relationship, relatedEntities?.Select(x => x?.ToEntityReference()));
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, EntityCollection entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            service.Associate(entities?.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, EntityCollection relatedEntities)
        {
            service.Associate(entities, relationship, relatedEntities?.Entities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate<T>(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity
        {
            service.Associate(entities, relationship, relatedEntities?.Select(x => x?.ToEntityReference()));
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
            where T : Entity
        {
            service.Associate(entities?.Select(x => x?.ToEntityReference()), relationship, relatedEntities);
        }

        /// <summary>
        /// Creates a link between records. Null references
        /// in the entities will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be associated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var reference in entities)
            {
                if (reference == null)
                {
                    Warning($"Found a Null reference in {nameof(entities)}. Skipping.");
                    continue;
                }

                service.Associate(reference, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The record to which the related records are associated.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            service.Associate(entity?.ToEntityReference(), relationship, relatedEntities);
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The record to which the related records are associated.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Associate(this IOrganizationService service, EntityReference entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (relatedEntities == null)
            {
                throw new ArgumentNullException(nameof(relatedEntities));
            }

            service.Associate(entity.LogicalName, entity.Id, relationship, new EntityReferenceCollection(relatedEntities.ToList()));
        }

        #endregion

        #region Disassociate

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, EntityCollection entities, Relationship relationship, EntityCollection relatedEntities)
        {
            service.Disassociate(entities.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, EntityCollection relatedEntities)
            where T : Entity, new()
        {
            service.Disassociate(entities, relationship, relatedEntities?.Entities);
        }


        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate<T>(this IOrganizationService service, EntityCollection entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity, new()
        {
            service.Disassociate(entities?.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity, new()
        {
            service.Disassociate(entities, relationship, relatedEntities?.Select(x => x?.ToEntityReference()));
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, EntityCollection entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            service.Disassociate(entities?.Entities, relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, EntityCollection relatedEntities)
        {
            service.Disassociate(entities, relationship, relatedEntities?.Entities);
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate<T>(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, IEnumerable<T> relatedEntities)
            where T : Entity
        {
            service.Disassociate(entities, relationship, relatedEntities?.Select(x => x?.ToEntityReference()));
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate<T>(this IOrganizationService service, IEnumerable<T> entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
            where T : Entity
        {
            service.Disassociate(entities?.Select(x => x?.ToEntityReference()), relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entities">Every record of this collection will be disassociated to every record in the <paramref name="relatedEntities"/>.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, IEnumerable<EntityReference> entities, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var reference in entities)
            {
                if (reference == null)
                {
                    Warning($"Found a Null reference in {nameof(entities)}. Skipping.");
                    continue;
                }

                service.Disassociate(reference, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The record to which the related records are associated.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be associated.</param>
        public static void Disassociate(this IOrganizationService service, Entity entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            service.Disassociate(entity?.ToEntityReference(), relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The record to which the related records are associated.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities"> A collection of entity references to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, EntityReference entity, Relationship relationship, IEnumerable<EntityReference> relatedEntities)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (relatedEntities == null)
            {
                throw new ArgumentNullException(nameof(relatedEntities));
            }

            service.Disassociate(entity.LogicalName, entity.Id, relationship, new EntityReferenceCollection(relatedEntities.ToList()));
        }

        #endregion

        #region Retrieve

        /// <summary>
        /// Retrieves a new instance of the record with ALL columns!
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <returns>The requested (newly created) entity instance having all non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, Entity entity)
        {
            return service.Retrieve(entity?.ToEntityReference());
        }

        /// <summary>
        /// Retrieves a new instance of the record with ALL columns!
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <returns>The requested (newly created) entity instance having all non-null columns.</returns>
        public static TEntity Retrieve<TEntity>(this IOrganizationService service, Entity entity) where TEntity : Entity
        {
            return service.Retrieve<TEntity>(entity?.ToEntityReference());
        }

        public static TEntity Retrieve<TEntity>(this IOrganizationService service, TEntity entity) where TEntity : Entity
        {
            return service.Retrieve<TEntity>(entity?.ToEntityReference());
        }

        public static TEntity Retrieve<TEntity>(this IOrganizationService service, TEntity entity, params string[] cols) where TEntity : Entity
        {
            return service.Retrieve<TEntity>(entity?.ToEntityReference(), new ColumnSet(cols));
        }

        public static TEntity Retrieve<TEntity>(this IOrganizationService service, TEntity entity, ColumnSet cols) where TEntity : Entity
        {
            return service.Retrieve<TEntity>(entity?.ToEntityReference(), cols);
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested (newly created) entity instance having the requested non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, Entity entity, ColumnSet cols)
        {
            return service.Retrieve(entity?.ToEntityReference(), cols);
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">All attributes to retrieve.</param>
        /// <returns>The requested (newly created) entity instance having the requested non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, Entity entity, params string[] cols)
        {
            return service.Retrieve(entity, new ColumnSet(cols));
        }

        /// <summary>
        /// Retrieves a record with ALL columns!
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <returns>The requested entity having all non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, EntityReference entity)
        {
            return service.Retrieve(entity, new ColumnSet(true));
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity having the requested non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, EntityReference entity, params string[] cols)
        {
            return service.Retrieve(entity, new ColumnSet(cols));
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity having the requested non-null columns.</returns>
        public static TEntity Retrieve<TEntity>(this IOrganizationService service, EntityReference entity, params string[] cols) where TEntity : Entity
        {
            return service.Retrieve<TEntity>(entity, new ColumnSet(cols));
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity having the requested non-null columns.</returns>
        public static TEntity Retrieve<TEntity>(this IOrganizationService service, EntityReference entity, ColumnSet cols) where TEntity : Entity
        {
            return service.Retrieve(entity, cols).ToEntity<TEntity>();
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to retrieve.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity having the requested non-null columns.</returns>
        public static Entity Retrieve(this IOrganizationService service, EntityReference entity, ColumnSet cols)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return service.Retrieve(entity.LogicalName, entity.Id, cols);
        }

        #endregion

        #region Deactivate

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, EntityCollection entitiesToChange, int status = -1)
        {
            service.Deactivate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, EntityCollection entitiesToChange, OptionSetValue status)
        {
            service.Deactivate(entitiesToChange?.Entities, status);
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, int status = -1)
            where T : Entity, new()
        {
            service.Deactivate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, OptionSetValue status)
            where T : Entity, new()
        {
            service.Deactivate(entitiesToChange?.Select(x => x?.ToEntityReference()), status);
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, int status = -1)
        {
            service.Deactivate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be deactivated.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, OptionSetValue status)
        {
            if (entitiesToChange == null)
            {
                throw new ArgumentNullException(nameof(entitiesToChange));
            }

            foreach (var entity in entitiesToChange)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(entitiesToChange)}. Skipping.");
                    continue;
                }

                service.Deactivate(entity, status);
            }
        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to deactivate.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, Entity entityToChange, int status = -1)
        {
            service.Deactivate(entityToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to deactivate.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, Entity entityToChange, OptionSetValue status)
        {
            service.Deactivate(entityToChange?.ToEntityReference(), status);
        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to deactivate.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, EntityReference entityToChange, int status = -1)
        {
            service.Deactivate(entityToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Deactivates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to deactivate.</param>
        /// <param name="status">The new status code.</param>
        public static void Deactivate(this IOrganizationService service, EntityReference entityToChange, OptionSetValue status)
        {
            service.SetState(entityToChange, new OptionSetValue(1), status);
        }

        #endregion

        #region Activate

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, EntityCollection entitiesToChange, int status = -1)
        {
            service.Activate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, EntityCollection entitiesToChange, OptionSetValue status)
        {
            service.Activate(entitiesToChange?.Entities, status);
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, int status = -1)
            where T : Entity, new()
        {
            service.Activate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, OptionSetValue status)
            where T : Entity, new()
        {
            service.Activate(entitiesToChange?.Select(x => x?.ToEntityReference()), status);
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, int status = -1)
        {
            service.Activate(entitiesToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates all records contained in the <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which shall be activated.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, OptionSetValue status)
        {
            if (entitiesToChange == null)
            {
                throw new ArgumentNullException(nameof(entitiesToChange));
            }

            foreach (var entity in entitiesToChange)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(entitiesToChange)}. Skipping.");
                    continue;
                }

                service.Activate(entity, status);
            }
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to activate.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, Entity entityToChange, int status = -1)
        {
            service.Activate(entityToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to activate.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, Entity entityToChange, OptionSetValue status)
        {
            service.Activate(entityToChange?.ToEntityReference(), status);
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to activate.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, EntityReference entityToChange, int status = -1)
        {
            service.Activate(entityToChange, new OptionSetValue(status));
        }

        /// <summary>
        /// Activates a record.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The records to activate.</param>
        /// <param name="status">The new status code.</param>
        public static void Activate(this IOrganizationService service, EntityReference entityToChange, OptionSetValue status)
        {
            service.SetState(entityToChange, new OptionSetValue(0), status);
        }

        #endregion

        #region SetState

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, EntityCollection entitiesToChange, int state, int status = -1)
        {
            service.SetState(entitiesToChange, new OptionSetValue(state), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, EntityCollection entitiesToChange, OptionSetValue state, OptionSetValue status)
        {
            service.SetState(entitiesToChange?.Entities, state, status);
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, int state, int status = -1)
            where T : Entity, new()
        {
            service.SetState(entitiesToChange, new OptionSetValue(state), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState<T>(this IOrganizationService service, IEnumerable<T> entitiesToChange, OptionSetValue state, OptionSetValue status)
            where T : Entity, new()
        {
            service.SetState(entitiesToChange?.Select(x => x?.ToEntityReference()), state, status);
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entitiesToChange">The records which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, int state, int status = -1)
        {
            service.SetState(entitiesToChange, new OptionSetValue(state), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the <paramref name="state"/> and <paramref name="status"/> of records contained in <paramref name="entitiesToChange"/>. Null references
        /// in the collection will be ignored.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, IEnumerable<EntityReference> entitiesToChange, OptionSetValue state, OptionSetValue status)
        {
            if (entitiesToChange == null)
            {
                throw new ArgumentNullException(nameof(entitiesToChange));
            }

            foreach (var entity in entitiesToChange)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(entitiesToChange)}. Skipping.");
                    continue;
                }

                service.SetState(entity, state, status);
            }
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, Entity entityToChange, int state, int status = -1)
        {
            service.SetState(entityToChange, new OptionSetValue(state), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, Entity entityToChange, OptionSetValue state, OptionSetValue status)
        {
            service.SetState(entityToChange?.ToEntityReference(), state, status);
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, EntityReference entityToChange, int state, int status = -1)
        {
            service.SetState(entityToChange, new OptionSetValue(state), new OptionSetValue(status));
        }

        /// <summary>
        /// Sets the record's <paramref name="state"/> and <paramref name="status"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityToChange">The record which status shall be set.</param>
        /// <param name="state">The new state code.</param>
        /// <param name="status">The new status code.</param>
        public static void SetState(this IOrganizationService service, EntityReference entityToChange, OptionSetValue state, OptionSetValue status)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var request = new SetStateRequest()
            {
                EntityMoniker = entityToChange,
                State = state,
                Status = status
            };

            service.Execute(request);
        }
        #endregion

        #region Globalization

        public static DateTime ToUserLocalDateTime(this IOrganizationService service, DateTime timeToConvert, Guid userId)
        {
            return service.ToUserLocalDateTime(timeToConvert, service.GetUserTimeZone(userId));
        }

        public static DateTime ToUserLocalDateTime(this IOrganizationService service, DateTime timeToConvert, int timeZoneCode)
        {
            return (service.Execute(new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode,
                UtcTime = timeToConvert,
            }) as LocalTimeFromUtcTimeResponse)?.LocalTime ?? throw new Exception($"Failed to convert current time to user local!");
        }

        public static int GetUserTimeZone(this IOrganizationService service, Guid userId)
        {
            return service.GetAll(new QueryExpression
            {
                EntityName= "usersettings",
                TopCount = 1,
                ColumnSet = new ColumnSet("timezonecode"),
                Criteria = new FilterExpression{ Conditions = { new ConditionExpression( "systemuserid", ConditionOperator.Equal, userId) } }
            })?.Entities?.FirstOrDefault()?.GetAttributeValue<int?>("timezonecode") ?? throw new Exception($"Failed to determine current users timezone!");
        }

        #endregion
    }
}
