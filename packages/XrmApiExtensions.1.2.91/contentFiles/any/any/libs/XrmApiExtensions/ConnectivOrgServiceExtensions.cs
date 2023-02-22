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

using Connectiv.XrmCommon;
using Connectiv.XrmCommon.Extensions.FetchXml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.Xrm.Sdk
{
    /// <summary>
    /// Provides additional helpful extension methods for the <see cref="IOrganizationService"/> interface.
    /// </summary>
    public static class ConnectivOrgServiceExtensions
    {
        /// <summary>
        /// Functionpointer which will be called to log debug messages.
        /// </summary>
        public static Action<string> Debug = x => { return; };

        /// <summary>
        /// Functionpointer which will be called to log info messages.
        /// </summary>
        public static Action<string> Info = x => { return; };

        /// <summary>
        /// Functionpointer which will be called to log warning messages.
        /// </summary>
        public static Action<string> Warning = x => { return; };

        /// <summary>
        /// Functionpointer which will be called to log error messages.
        /// </summary>
        public static Action<string> Error = x => { return; };

        /// <summary>
        /// Functionpointer which will be called to send heart beats.
        /// </summary>
        public static Action HeartBeat = () => { return; };


        private static IOrganizationService organizationService = null;

        /// <summary>
        /// CRM Service instance that will be used by other extension methods. For example
        /// so you can call the Update method on an EntityCollection without
        /// providing a service the whole time.
        /// </summary>
        public static IOrganizationService OrganizationService
        {
            get
            {
                return organizationService;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(OrganizationService));
                }

                organizationService = value;
            }
        }

        #region Upsert
        public static EntityUpsertResponse<TEntity> Upsert<TEntity>(this IOrganizationService service, TEntity entity) where TEntity : Entity
        {
            var response = (service?.Execute(new UpsertRequest { Target = entity }) as UpsertResponse);
            entity.Id = response.Target.Id;
            return new EntityUpsertResponse<TEntity>(entity, response.RecordCreated ? UpsertResponseState.Created : UpsertResponseState.Updated);
        }
        #endregion

        #region GetAllActive

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, bool allColumns = false, bool noLock = false)
        {
            return service.GetAllActive(entityName, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, ColumnSet columnSet, bool noLock = false)
        {
            return service.GetAllActive(entityName, new FilterExpression(), columnSet, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// only the primary field.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, ConditionExpression condition, bool noLock = false)
        {
            return service.GetAllActive(entityName, condition, new ColumnSet(false), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, ConditionExpression condition, ColumnSet columnSet, bool noLock = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return HandleExceedingConditions(service.GetAllActive, entityName, condition, columnSet, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, FilterExpression filter, bool allColums = false, bool noLock = false)
        {
            return service.GetAllActive(entityName, filter, new ColumnSet(allColums), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, string entityName, FilterExpression filter, ColumnSet columnSet, bool noLock = false)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            var query = new QueryExpression(entityName);
            query.ColumnSet = columnSet;
            query.Criteria.Filters.Add(filter);

            return service.GetAllActive(query, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="query"/>. Returned records contain
        /// the attributes specified in the query's <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="query">Base query to which the active statecode condition will be added.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllActive(this IOrganizationService service, QueryExpression query, bool noLock = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));

            return service.GetAll(query, noLock);
        }

        #endregion

        #region GetAllInactive

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, bool allColumns = false, bool noLock = false)
        {
            return service.GetAllInActive(entityName, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, ColumnSet columnSet, bool noLock = false)
        {
            return service.GetAllInActive(entityName, new FilterExpression(), columnSet, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) active records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, ConditionExpression condition, bool allColumns = false, bool noLock = false)
        {
            return service.GetAllInActive(entityName, condition, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, ConditionExpression condition, ColumnSet columnSet, bool noLock = false)
        {
            return HandleExceedingConditions(service.GetAllInActive, entityName, condition, columnSet, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, FilterExpression filter, bool allColums = false, bool noLock = false)
        {
            return service.GetAllInActive(entityName, filter, new ColumnSet(allColums), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, string entityName, FilterExpression filter, ColumnSet columnSet, bool noLock = false)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            var query = new QueryExpression(entityName);
            query.ColumnSet = columnSet;
            query.Criteria.Filters.Add(filter);

            return service.GetAllInActive(query, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) inactive records of entity <paramref name="entityName"/> and additionally applies <paramref name="query"/>. Returned records contain
        /// the attributes specified in the query's <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="query">Base query to which the active statecode condition will be added.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found inactive records.</returns>
        public static EntityCollection GetAllInActive(this IOrganizationService service, QueryExpression query, bool noLock = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Inactive"));

            return service.GetAll(query, noLock);
        }

        #endregion

        #region GetAllEntities

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, bool allColumns = false, bool noLock = false)
        {
            return service.GetAll(entityName, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, ColumnSet columnSet, bool noLock = false)
        {
            return service.GetAll(entityName, new FilterExpression(), columnSet, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, ConditionExpression condition, bool allColumns = false, bool noLock = false)
        {
            return service.GetAll(entityName, condition, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="condition"/> is null.</exception>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, ConditionExpression condition, ColumnSet columnSet, bool noLock = false)
        {
            return HandleExceedingConditions(service.GetAll, entityName, condition, columnSet, noLock);
        }

        public static EntityCollection GetAll(this IOrganizationService service, string entityName, ConditionExpression condition, bool noLock = false, params string[] cols)
        {
            return service.GetAll(entityName, condition, new ColumnSet(cols), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, FilterExpression filter, bool allColumns = false, bool noLock = false)
        {
            return service.GetAll(entityName, filter, new ColumnSet(allColumns), noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, string entityName, FilterExpression filter, ColumnSet columnSet, bool noLock = false)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            var qex = new QueryExpression(entityName);
            qex.ColumnSet = columnSet;
            qex.Criteria.AddFilter(filter);

            return service.GetAll(qex, noLock);
        }

        /// <summary>
        /// Queries all (5000+ possible) records applying to <paramref name="queryExpr"/> and returns them.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="queryExpr">Query to execute. If ColumnSet property is null, only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static EntityCollection GetAll(this IOrganizationService service, QueryExpression queryExpr, bool noLock = false)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (queryExpr == null)
            {
                throw new ArgumentNullException(nameof(queryExpr));
            }

            var allEntities = new EntityCollection();

            queryExpr.ColumnSet = queryExpr.ColumnSet ?? new ColumnSet(false);
            if (noLock) queryExpr.NoLock = noLock;

            int pageNumber = 1;
            
            do
            {
                var resp = service.RetrieveMultiple(queryExpr);

                allEntities.Entities.AddRange(resp.Entities.ToArray());

                HeartBeat();
                if (!resp.MoreRecords)
                {
                    break;
                }

                queryExpr.PageInfo.PagingCookie = resp.PagingCookie;
                queryExpr.PageInfo.PageNumber = ++pageNumber;
                queryExpr.PageInfo.Count = 5000; // In CRM Version 9.2.20122.144 not setting this value, will result in getting the first page over and over, even though we are increasing the pagenumber.
                Debug($"Found {resp.Entities.Count} more records. Current total: {allEntities.Entities.Count}");
            } while (true);

            Debug($"[{allEntities.Entities.Count}] {queryExpr.EntityName}s found!");

            return allEntities;
        }

        #endregion

        #region GetAll Generic

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, bool allColumns = false, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, new ColumnSet(allColumns), noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found active records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, ColumnSet columnSet, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, columnSet, noLock));
        }

        public static List<T> GetAll<T>(this IOrganizationService service, bool noLock = false, params String[] columns)
    where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, new ColumnSet(columns), noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, ConditionExpression condition, bool allColumns = false, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, condition, allColumns, noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="condition"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// <para/>
        /// If the <paramref name="condition"/> contains more than 2000 values the CRM would throw an error. This method creates smaller chunks before and executes them.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="condition"/> is null.</exception>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, ConditionExpression condition, ColumnSet columnSet, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, condition, columnSet, noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// either all attributes or only the primary field.
        /// WARN: Changed default value of allColumns to false.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="allColumns">If true query returns all attributes, else only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, FilterExpression filter, bool allColumns = false, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, filter, allColumns, noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records of entity <paramref name="entityName"/> and additionally applies <paramref name="filter"/>. Returned records contain
        /// the attributes specified in <paramref name="columnSet"/>.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, FilterExpression filter, ColumnSet columnSet, bool noLock = false)
            where T : Entity, new()
        {
            return ConvertToTypedEntity<T>(service.GetAll(new T().LogicalName, filter, columnSet, noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records applying to <paramref name="queryExpr"/> and returns them.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="queryExpr">Query to execute. If ColumnSet property is null, only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, QueryExpression queryExpr, bool noLock = false)
            where T : Entity, new()
        {
            if(String.IsNullOrWhiteSpace(queryExpr.EntityName))
            {
                queryExpr.EntityName = new T().LogicalName;
            }

            return ConvertToTypedEntity<T>(service.GetAll(queryExpr, noLock));
        }

        /// <summary>
        /// Queries all (5000+ possible) records applying to <paramref name="fetch"/> and returns them
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="service">CRM service instance.</param>
        /// <param name="fetch">Query to execute. Not allowed to contain any paging information yet!</param>
        /// <returns>List of all found records.</returns>
        public static List<T> GetAll<T>(this IOrganizationService service, string fetch)
            where T : Entity, new()
        {
            List<T> retVal = new List<T>();

            EntityCollection all_Entities;

            // Set the number of records per page to retrieve.
            int fetchCount = 5000;
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            do
            {
                string xml = CreateXml(fetch, pagingCookie, pageNumber, fetchCount);

                all_Entities = service.RetrieveMultiple(new FetchExpression(xml));

                retVal.AddRange(ConvertToTypedEntity<T>(all_Entities));

                pageNumber++;
                pagingCookie = all_Entities.PagingCookie;

                HeartBeat();
            }
            while (all_Entities.MoreRecords);

            return retVal;
        }

        #endregion

        #region CountAll

        /// <summary>
        /// Returns the total number of records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of records.</returns>
        public static int CountAll(this IOrganizationService service, string entityName, bool noLock = false)
        {
            return service.GetAll(entityName, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of records matching the <paramref name="condition"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of records.</returns>
        public static int CountAll(this IOrganizationService service, string entityName, ConditionExpression condition, bool noLock = false)
        {
            return service.GetAll(entityName, condition, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of records matching the <paramref name="filter"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional filter which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of records.</returns>
        public static int CountAll(this IOrganizationService service, string entityName, FilterExpression filter, bool noLock = false)
        {
            return service.GetAll(entityName, filter, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of records.</returns>
        public static int CountAll(this IOrganizationService service, QueryExpression query, bool noLock = false)
        {
            return service.GetAll(query, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of active records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllActive(this IOrganizationService service, string entityName, bool noLock = false)
        {
            return service.GetAllActive(entityName, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of active records matching the <paramref name="condition"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllActive(this IOrganizationService service, string entityName, ConditionExpression condition, bool noLock = false)
        {
            return service.GetAllActive(entityName, condition, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of active records matching the <paramref name="filter"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional filter which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllActive(this IOrganizationService service, string entityName, FilterExpression filter, bool noLock = false)
        {
            return service.GetAllActive(entityName, filter, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of active records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllActive(this IOrganizationService service, QueryExpression query, bool noLock = false)
        {
            return service.GetAllActive(query, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of inactive records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of inactive records.</returns>
        public static int CountAllInActive(this IOrganizationService service, string entityName, bool noLock = false)
        {
            return service.GetAllInActive(entityName, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of inactive records matching the <paramref name="condition"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of inactive records.</returns>
        public static int CountAllInActive(this IOrganizationService service, string entityName, ConditionExpression condition, bool noLock = false)
        {
            return service.GetAllInActive(entityName, condition, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of inactive records matching the <paramref name="filter"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="filter">Additional filter which will be added to the query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllInActive(this IOrganizationService service, string entityName, FilterExpression filter, bool noLock = false)
        {
            return service.GetAllInActive(entityName, filter, noLock).Entities.Count;
        }

        /// <summary>
        /// Returns the total number of inactive records of type <paramref name="entityName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>Total number of active records.</returns>
        public static int CountAllInActive(this IOrganizationService service, QueryExpression query, bool noLock = false)
        {
            return service.GetAllInActive(query, noLock).Entities.Count;
        }

        #endregion

        #region FetchToQuery-QueryToFetch

        /// <summary>
        /// Takes the <paramref name="fetchXML"/> and converts it to a <see cref="QueryExpression"/> instance
        /// unsing the CRM <paramref name="service"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="fetchXML">A valid FetchXml that shall be converted to a QueryExpression.</param>
        /// <returns>The converted <paramref name="fetchXML"/> as <see cref="QueryExpression"/> instance.</returns>
        public static QueryExpression ConvertFetchToQuery(this IOrganizationService service, string fetchXML)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            try
            {
                var conversionRequest = new FetchXmlToQueryExpressionRequest
                {
                    FetchXml = fetchXML
                };

                var conversionResponse = (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

                return conversionResponse.Query;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during conversion of FetchXML to QueryExpression. Original FetchXml: {Environment.NewLine} {fetchXML} .", ex);
            }
        }

        /// <summary>
        /// Returns the count of results to expect when executing the query.
        /// </summary>
        /// <param name="fetchXml"></param>
        /// <returns></returns>
        public static int GetCountAggregate(this IOrganizationService service, FetchXmlExpression fetchXml)
        {
            var countAggregate = fetchXml.ConvertToCountAggregation();

            EntityCollection all_Entities;

            // Set the number of records per page to retrieve.
            int fetchCount = 5000;
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            string xml = CreateXml(countAggregate.RawXml, pagingCookie, pageNumber, fetchCount);

            all_Entities = service.RetrieveMultiple(new FetchExpression(xml));

            if (all_Entities.Entities.Count > 0 && all_Entities.Entities.First().Contains("totalcount"))
            {
                return int.Parse((all_Entities.Entities.First()["totalcount"] as AliasedValue).Value.ToString());
            }

            return -1;
        }

        /// <summary>
        /// Takes the <paramref name="query"/> and converts it to a FetchXml using the CRM <paramref name="service"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="query">The query that shall be converted to a FetchXml.</param>
        /// <returns>The converted <paramref name="query"/> instance as a FetchXml.</returns>
        public static string ConvertQueryToFetch(this IOrganizationService service, QueryExpression query)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            try
            {
                var conversionRequest = new QueryExpressionToFetchXmlRequest
                {
                    Query = query
                };

                var conversionResponse = (QueryExpressionToFetchXmlResponse)service.Execute(conversionRequest);

                return conversionResponse.FetchXml;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during conversion of QueryExpression to FetchXML.", ex);
            }
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Refreshes all the attributes for the <paramref name="entitiesToRefresh"/>. Creates a new Entity instance for the entityreference. 
        /// Adds all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entityToRefresh">Entity reference which attributes shall you want to refresh. This instance will be newly created. Attributes will be added having the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity instances having all non-null columns.</returns>
        public static Entity Refresh(this IOrganizationService service, EntityReference entityToRefresh, ColumnSet cols)
        {
            return service.Refresh(entityToRefresh.ToEntity(), cols);
        }

        /// <summary>
        /// Refreshes all the attributes for each entity reference in <paramref name="entitiesToRefresh"/>. Creates a new Entity instance for each element in the list.
        /// Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entitiesToRefresh">List of entitiy references which attributes shall you want to refresh. The instances will be newly created. Attributes will be added having the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity instances having all non-null columns.</returns>
        public static IEnumerable<Entity> Refresh(this IOrganizationService service, IEnumerable<EntityReference> entitiesToRefresh, ColumnSet cols)
        {
            return service.Refresh(entitiesToRefresh?.Select(x => new Entity(x.LogicalName, x.Id)), cols);
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="entitiesToRefresh"/>. Updates all attributes already set in the record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entitiesToRefresh">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <returns>The requested entity instances (same as in params) having all non-null columns.</returns>
        public static EntityCollection Refresh(this IOrganizationService service, EntityCollection entitiesToRefresh)
        {
            service.Refresh(entitiesToRefresh?.Entities);
            return entitiesToRefresh;
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="entitiesToRefresh"/>. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entitiesToRefresh">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity instances (same as in params) having all non-null columns.</returns>
        public static EntityCollection Refresh(this IOrganizationService service, EntityCollection entitiesToRefresh, ColumnSet cols)
        {
            service.Refresh(entitiesToRefresh?.Entities, cols);
            return entitiesToRefresh;
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="entitiesToRefresh"/>. Updates all attributes already set in the record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entitiesToRefresh">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <returns>The requested entity instances (same as in params) having all non-null columns.</returns>
        public static IEnumerable<T> Refresh<T>(this IOrganizationService service, IEnumerable<T> entitiesToRefresh)
            where T : Entity, new()
        {
            foreach (var entity in entitiesToRefresh)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(entitiesToRefresh)}. Skipping.");
                    continue;
                }

                service.Refresh(entity);
            }

            return entitiesToRefresh;
        }

        /// <summary>
        /// Refreshes all the attributes for each entity in <paramref name="entitiesToRefresh"/>. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entitiesToRefresh">List of entities which attributes shall you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity instances (same as in params) having all non-null columns.</returns>
        public static IEnumerable<T> Refresh<T>(this IOrganizationService service, IEnumerable<T> entitiesToRefresh, ColumnSet cols)
            where T : Entity, new()
        {
            foreach (var entity in entitiesToRefresh)
            {
                if (entity == null)
                {
                    Warning($"Found a Null reference in {nameof(entitiesToRefresh)}. Skipping.");
                    continue;
                }

                service.Refresh(entity, cols);
            }

            return entitiesToRefresh;
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Updates all attributes already set in the record.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <returns>The requested entity instance (same as in params) having all non-null columns.</returns>
        public static Entity Refresh(this IOrganizationService service, Entity entity)
        {
            return service.Refresh(entity, new ColumnSet(entity.Attributes.Keys.ToArray()));
        }

        /// <summary>
        /// Refreshes the <paramref name="entity"/>'s attributes. Adds or updates all retrieved <paramref name="cols"/> within the attribute's list.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="entity">The ID and logicalname of the record that you want to refresh. This instance will also be returned after adding/updating the attributes with the latest values.</param>
        /// <param name="cols">A query that specifies the set of attributes to retrieve.</param>
        /// <returns>The requested entity instance (same as in params) having all non-null columns.</returns>
        public static Entity Refresh(this IOrganizationService service, Entity entity, ColumnSet cols)
        {
            var retVal = service.Retrieve(entity, cols);

            foreach (var attribute in retVal.Attributes)
            {
                entity.Attributes[attribute.Key] = attribute.Value;
            }

            return entity;
        }

        #endregion

        #region AssociationExist

        public static bool IsRelationshipAlreadyExist(this IOrganizationService service, string relationshipname, Guid entity1Id, string entity1Name, Guid entity2Id, string entity2Name)
        {
            string relationship1EtityName = string.Format("{0}id", entity1Name);
            string relationship2EntityName = string.Format("{0}id", entity2Name);

            //This check is added for self-referenced relationships
            if (entity1Name.Equals(entity2Name, StringComparison.InvariantCultureIgnoreCase))
            {
                relationship1EtityName = string.Format("{0}idone", entity1Name);
                relationship1EtityName = string.Format("{0}idtwo", entity1Name);
            }

            QueryExpression query = new QueryExpression(entity1Name) { ColumnSet = new ColumnSet(false) };

            LinkEntity link = query.AddLink(relationshipname, string.Format("{0}id", entity1Name), relationship1EtityName);

            link.LinkCriteria.AddCondition(relationship1EtityName, ConditionOperator.Equal, new object[] { entity1Id });

            link.LinkCriteria.AddCondition(relationship2EntityName, ConditionOperator.Equal, new object[] { entity2Id });

            return service.RetrieveMultiple(query).Entities.Count > 0;
        }

        #endregion

        #region Team Helper

        /// <summary>
        /// Returns a distinc list of all systemuser who are member of at least one of the given <paramref name="teamNames"/>.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="teamNames">Members of this teams will be returned.</param>
        /// <param name="cols">List of fields of system users.</param>
        /// <returns>Distinct list of found systemuser.</returns>
        public static IEnumerable<Entity> GetTeamMembers(this IOrganizationService service, IEnumerable<string> teamName)
        {
            return service.GetTeamMembers(teamName, new ColumnSet(false));
        }

        /// <summary>
        /// Returns a distinc list of all systemuser who are member of at least one of the given <paramref name="teamNames"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="cols"/> is null.</exception>
        /// <exception cref="ArgumentException">If found teams count is null or does not equal amount of requested teams.</exception>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="teamNames">Members of this teams will be returned.</param>
        /// <param name="cols">List of fields of system users.</param>
        /// <returns>Distinct list of found systemuser.</returns>
        public static IEnumerable<Entity> GetTeamMembers(this IOrganizationService service, IEnumerable<string> teamNames, ColumnSet cols)
        {
            QueryExpression teamQuery = new QueryExpression("team");
            teamQuery.ColumnSet = new ColumnSet(false);
            teamQuery.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.In, teamNames));

            var teams = service.RetrieveMultiple(teamQuery);
            Debug($"Found {teams.Entities.Count} teams");

            if (teams.Entities.Count == 0)
            {
                throw new ArgumentException($"No team found with one of the following names:  {string.Join(", ", teamNames)}!");
            }
            else if (teams.Entities.Count != teamNames.Count())
            {
                throw new ArgumentException($"Retrieved {teams.Entities.Count} teams, but requested were {teamNames.Count()}. At least one of the following names does not exist: {string.Join(", ", teamNames)}.");
            }

            return service.GetTeamMembers(teams.Entities.Select(x => x.Id).ToArray(), cols);
        }

        /// <summary>
        /// Returns a distinc list of all systemuser who are member of at least one of the given <paramref name="teamNames"/>.
        /// </summary>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="teamIds">Members of this teams will be returned.</param>
        /// <param name="cols">List of fields of system users.</param>
        /// <returns>Distinct list of found systemuser.</returns>
        public static IEnumerable<Entity> GetTeamMembers(this IOrganizationService service, IEnumerable<Guid> teamIds)
        {
            return service.GetTeamMembers(teamIds, new ColumnSet(false));
        }

        /// <summary>
        /// Returns a distinc list of all systemuser who are member of at least one of the given <paramref name="teamIds"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="cols"/> is null.</exception>
        /// <param name="service">CRM Service Instance.</param>
        /// <param name="teamIds">Members of this teams will be returned.</param>
        /// <param name="cols">List of fields of system users.</param>
        /// <returns>Distinct list of found systemuser.</returns>
        public static IEnumerable<Entity> GetTeamMembers(this IOrganizationService service, IEnumerable<Guid> teamIds, ColumnSet cols)
        {
            if (teamIds.Count() == 0)
            {
                Warning($"Parameter {nameof(teamIds)} does not contain any items to query. Returning empty list.");
                return new List<Entity>();
            }

            if (cols == null)
            {
                throw new ArgumentNullException(nameof(cols));
            }

            var totalMembers = new List<Entity>();

            QueryExpression userQuery = new QueryExpression("systemuser");
            userQuery.ColumnSet = cols;

            LinkEntity teamLink = new LinkEntity("systemuser", "teammembership", "systemuserid", "systemuserid", JoinOperator.Inner);

            // Ask Microsoft why this only works with an object[]... Some hours it was working  ago also with an Guid[]!
            teamLink.LinkCriteria.AddCondition(new ConditionExpression("teamid", ConditionOperator.In, teamIds.OfType<object>().ToArray()));

            userQuery.LinkEntities.Add(teamLink);

            // List is already distinct by CRM
            EntityCollection retrievedUsers = service.RetrieveMultiple(userQuery);

            Debug($"Found {retrievedUsers.Entities.Count} users.");

            return retrievedUsers.Entities;
        }

        #endregion

        #region Config Helper

        /// <summary>
        /// Loading the required con_config values and writes them into a dictionary.
        /// Key is the con_name and the value is the con_value.
        /// </summary>
        /// <exception cref="InvalidPluginExecutionException">Will be thrown if not all required con_configs could be retrieved.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="configNames">List of all con_config records to return.</param>
        /// <returns>Returns a dictionary for the con_configs with the name as key and the value as the value.</returns>
        public static Dictionary<string, string> LoadConConfigValues(this IOrganizationService service, params string[] configNames)
        {
            var conConfigs = new Dictionary<string, string>();

            if (configNames.Length == 0)
            {
                Warning($"No config name given. Returning an empty dictionary.");
                return conConfigs;
            }

            QueryExpression query = new QueryExpression("con_config")
            {
                ColumnSet = new ColumnSet("con_value", "con_name"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("con_name", ConditionOperator.In, configNames),
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                    }
                }
            };

            var entities = service.RetrieveMultiple(query).Entities;

            if (entities.Count() > configNames.Count())
            {
                throw new InvalidPluginExecutionException($"{configNames.Count()} aktive con_config Einträge erwartet, aber {entities.Count()} gefunden: {String.Join(",", entities.Select(x => x.GetAttributeValue<string>("con_name")).OrderBy(x => x))}");
            }

            foreach (var name in configNames)
            {
                conConfigs.Add(name, entities.FirstOrDefault(x => x.GetAttributeValue<string>("con_name").Equals(name))?.GetAttributeValue<string>("con_value"));
            }

            var emptyConfigs = conConfigs.Where(x => x.Value == null);

            if (emptyConfigs.Count() > 0)
            {
                throw new InvalidPluginExecutionException($"Folgende con_configs Einträge fehlen: {String.Join(",", emptyConfigs.Select(x => x.Key))}");
            }

            return conConfigs;
        }

        #endregion

        #region Optionset Helper

        /// <summary>
        /// Gets the label of the <paramref name="optionSetValue"/> in the configured language code. If the language code is not available the user localized label will be returned.
        /// Returns an empty string, if there is no option having the requested <paramref name="optionSetValue"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="optionSetValue"/> is null.</exception>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Logical name of the entity the <paramref name="optionSetLogicalName"/>.</param>
        /// <param name="optionSetLogicalName">Logical name of the optionset attribute.</param>
        /// <param name="languageCode">The code determining the language to get the label in.</param>
        /// <param name="optionSetValue">Value of the option to get the label from.</param>
        /// <returns>Returns the label of the optionset or string.Empty if the label can not be determined.</returns>
        public static string GetOptionSetValueLabel(this IOrganizationService service, string entityName, string optionSetLogicalName, int languageCode, OptionSetValue optionSetValue)
        {
            string toReturn = string.Empty;

            OptionMetadata option = service.GetOptionSetValueMetadata(entityName, optionSetLogicalName, optionSetValue);

            if (option == null)
            {
                Warning($"No option with value eq {optionSetValue.Value} found. Returning string.Empty.");
                return string.Empty;
            }

            Debug($"Language code is set to {languageCode}. Trying to get label for the option {optionSetValue.Value}...");
            toReturn = option.Label.LocalizedLabels.FirstOrDefault(x => x.LanguageCode == languageCode)?.Label;
            Debug($"Language code is set to {languageCode}. Trying to get label for the option {optionSetValue.Value}...Done. Label is \"{toReturn ?? "null"}\".");

            if (string.IsNullOrWhiteSpace(toReturn))
            {
                Warning($"No localized label found for requested languagecode {languageCode}!");
                Debug($"Trying to get the user localized label (default)...");
                toReturn = option.Label.UserLocalizedLabel?.Label ?? string.Empty;
                Debug($"Trying to get the user localized label (default)...Done. The label is \"{toReturn}\"");
            }

            Debug($"Returning label \"{toReturn}\"");
            return toReturn;
        }

        /// <summary>
        /// Returns the metadata information about one option of the <paramref name="optionSetLogicalName"/> or
        /// null if the optionset does not contain an option having the requested value.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="optionSetValue"/> is null.</exception>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Logical name of the entity the <paramref name="optionSetLogicalName"/>.</param>
        /// <param name="optionSetLogicalName">Logical name of the optionset attribute.</param>
        /// <param name="optionSetValue">Value of the option to get the metadata from.</param>
        /// <returns>The queried metadata of a single option.</returns>
        public static OptionMetadata GetOptionSetValueMetadata(this IOrganizationService service, string entityName, string optionSetLogicalName, OptionSetValue optionSetValue)
        {
            if (optionSetValue == null)
            {
                throw new ArgumentNullException(nameof(optionSetValue), "Please specify a non-null optionset value!");
            }

            var attributeMetadata = service.GetOptionSetMetadata(entityName, optionSetLogicalName);

            var option = attributeMetadata.OptionSet.Options.FirstOrDefault(x => x.Value == optionSetValue.Value);
            return option;
        }

        /// <summary>
        /// Returns the metadata information about the <paramref name="optionSetLogicalName"/>.
        /// </summary>
        /// <param name="service">CRM service instance.</param>
        /// <param name="entityName">Logical name of the entity the <paramref name="optionSetLogicalName"/>.</param>
        /// <param name="optionSetLogicalName">Logical name of the optionset attribute.</param>
        /// <returns>The queried optionset metadata.</returns>
        public static EnumAttributeMetadata GetOptionSetMetadata(this IOrganizationService service, string entityName, string optionSetLogicalName)
        {
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = optionSetLogicalName,
                RetrieveAsIfPublished = true,
            };

            Debug($"Begin to retrieve option set metadata for {optionSetLogicalName} from the entity {entityName}...");
            var attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
            Debug($"Begin to retrieve option set metadata for {optionSetLogicalName} from the entity {entityName}...Done.");
            return (EnumAttributeMetadata)attributeResponse.AttributeMetadata;
        }

        #endregion

        #region Email Helper

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> to)
        {
            return service.SendEMail(subject, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, EntityReference from, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, EntityReference from, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, EntityReference from, IEnumerable<EntityReference> to)
        {
            return service.SendEMail(subject, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, EntityReference from, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, from, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> from, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> from, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, from, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> from, IEnumerable<EntityReference> to)
        {
            return service.SendEMail(subject, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, IEnumerable<EntityReference> from, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            return service.SendEMail(subject, string.Empty, from, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, description, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, description, new EntityReference[0], to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<EntityReference> to)
        {
            return service.SendEMail(subject, description, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards. Sender will be the user calling this method.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            return service.SendEMail(subject, description, new EntityReference[0], to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, EntityReference from, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, description, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, EntityReference from, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, description, new EntityReference[] { from }, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, EntityReference from, IEnumerable<EntityReference> to)
        {
            return service.SendEMail(subject, description, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from"> Sender of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, EntityReference from, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            return service.SendEMail(subject, description, new EntityReference[] { from }, to, regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<EntityReference> from, IEnumerable<Entity> to)
        {
            return service.SendEMail(subject, description, from, to, new EntityReference());
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to.</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<EntityReference> from, IEnumerable<Entity> to, EntityReference regarding)
        {
            return service.SendEMail(subject, description, from, to.Select(x => x.ToEntityReference()), regarding);
        }

        /// <summary>
        /// Creates an email record and immediatly sends it afterwards.
        /// </summary>
        /// <exception cref="ArgumentNullException">If either <paramref name="subject"/> or <paramref name="to"/> is null.</exception>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="subject">Subject to set for the email (required).</param>
        /// <param name="description">Text for the email body (optional).</param>
        /// <param name="from">List of senders of the email (optional). If empty or null the from attribute will not be set. Sender will be the user calling this method.</param>
        /// <param name="to">List of recipients (required).</param>
        /// <param name="regarding">Reference to the record the email will be linked to (optional).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, string subject, string description, IEnumerable<EntityReference> from, IEnumerable<EntityReference> to, EntityReference regarding)
        {
            if (to == null || to.Count(x => x.Id != Guid.Empty) <= 0)
            {
                throw new ArgumentNullException(nameof(to), "You must specify at least one recipient having a valid id!");
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject), "You must specify the subject!");
            }

            var email = new Entity("email");
            email["subject"] = subject;
            email["description"] = description;
            email["to"] = to.ToActivityParty();
            email["directioncode"] = true; // Outgoing

            if (regarding != null && regarding.Id != Guid.Empty)
            {
                email["regardingobjectid"] = regarding;
            }

            if (from != null && from.Count(x => x.Id != Guid.Empty) > 0)
            {
                email["from"] = from.ToActivityParty();
            }

            Debug("Begin to create email...");
            email.Id = service.Create(email);
            Debug($"Begin to create email...Done. Emailid: {email.Id}.");

            return service.SendEMail(email);
        }

        /// <summary>
        /// Sends the <paramref name="email"/>. If the <paramref name="email"/>
        /// has an empty ID, the record will be created first.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="email">Email to send (will be created if instance has an empty id).</param>
        /// <returns>Id of the sent mail.</returns>
        public static Guid SendEMail(this IOrganizationService service, Entity email)
        {
            if (email.Id == Guid.Empty)
            {
                Info("Given email does not have an id, creating the email record!");
                email.Id = service.Create(email);
                Info($"Created email: {email.Id}");
            }

            var req = new SendEmailRequest
            {
                EmailId = email.Id,
                IssueSend = true
            };

            Debug("Begin to send email...");
            service.Execute(req);
            Debug("Begin to send email...Done.");
            return email.Id;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Takes the given <paramref name="xml"/> and enriches it with paging-info.
        /// </summary>
        /// <param name="xml">Complete xml document.</param>
        /// <param name="cookie">The paging-cookie indicating last found record.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="count">The amount of records per page to retrieve.</param>
        /// <returns>FetchXml having additional information about paging.</returns>
        private static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        /// <summary>
        /// Converts the entities of the <paramref name="collection"/> to the strongly typed class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A strongly typed entity class.</typeparam>
        /// <param name="collection">List of entity records to convert.</param>
        /// <returns>The converted records.</returns>
        private static List<T> ConvertToTypedEntity<T>(EntityCollection collection)
            where T : Entity, new()
        {
            return collection.Entities.Select(x => x.ToEntity<T>()).ToList();
        }

        /// <summary>
        /// Splits up the values of a <see cref="ConditionExpression"/> if these exceed a number greater 2000 and executes the <paramref name="callBack"/>
        /// with a new condition having at most 1999 values. This is required because the CRM would throw an error otherwise. Reason why this works is,
        /// that all values within the condition are OR-connected.
        /// The results will be collected and returned.
        /// </summary>
        /// <param name="callBack">Functionpointer that executes the service request with the </param>
        /// <param name="entityName">Internal name of the entity to query.</param>
        /// <param name="condition">Additional condition which will be added to the query.</param>
        /// <param name="columnSet">List of attributes that are returned. If null only ID field will be returned.</param>
        /// <param name="noLock">Please see Please see <see cref="QueryExpression.NoLock"/>.</param>
        /// <returns>List of all found records.</returns>
        private static EntityCollection HandleExceedingConditions(Func<string, FilterExpression, ColumnSet, bool, EntityCollection> callBack, string entityName, ConditionExpression condition, ColumnSet columnSet, bool noLock = false)
        {
            List<List<Object>> splittedValues = condition.Values.Split<Object>(1999);

            // Condition contained no vaules (e.g. with operator 'NotNull') - so add one entry so that the request will at least be executed once
            if (splittedValues.Count == 0)
            {
                splittedValues.Add(null);
            }

            EntityCollection allEntities = new EntityCollection();
            allEntities.EntityName = entityName;

            foreach (List<Object> values in splittedValues)
            {
                var filter = new FilterExpression();
                filter.AddCondition(new ConditionExpression(condition.AttributeName, condition.Operator, values));

                allEntities.Entities.AddRange(callBack(entityName, filter, columnSet, noLock).Entities);
            }

            return allEntities;
        }

        #endregion

        #region Execute
        public static TResponse Execute<TResponse>(this IOrganizationService service, OrganizationRequest request) where TResponse : OrganizationResponse, new()
        {
            // somehow the crm doesnt works well with generated classes even when the EnableProxyTypes() is set - so we "unpack" the request itself before sending
            OrganizationResponse response = service.Execute(new OrganizationRequest
            {
                RequestName = request.RequestName,
                Parameters = request.Parameters,
                RequestId = request.RequestId,
                ExtensionData = request.ExtensionData
            });
            // and "pack" it again before returning
            return new TResponse
            {
                Results = response.Results,
                ExtensionData = response.ExtensionData
            };
        }

        #endregion

        #region BulkCreate

        /// <summary>
        /// Creates new CRM records for each item in <paramref name="recordsToImport"/> using the csv data import.
        /// </summary>
        /// <param name="service">CRM Service instance.</param>
        /// <param name="recordsToImport">List of all records to parse as csv file.</param>
        /// <param name="attributes">Key is HEADER for csv. Value will be evaluated for each record. For details see <see cref="EntityCollectionExtensions.GenerateCsv(IEnumerable{Entity}, Dictionary{string, string})"/>.</param>
        /// <param name="enableDuplicateDetection">Enable or disable the duplicate detection during import.</param>
        /// <returns>The ID of the asynchronous import records job.</returns>
        public static Guid BulkCreateUsingCsvImport(this IOrganizationService service, IEnumerable<Entity> recordsToImport, Dictionary<string, string> attributes, bool enableDuplicateDetection = false)
        {
            Debug($"Generating CSV...");

            if (recordsToImport == null || !recordsToImport.Any(x => x != null))
            {
                return Guid.Empty;
            }

            StringBuilder csvBuilder = recordsToImport.GenerateCsv(attributes);

            Debug($"Generating CSV...DONE");
            Debug(csvBuilder.ToString());

            var targetLogicalName = recordsToImport.First().LogicalName;

            Debug($"Creating import in CRM...");
            Entity import = new Entity("import");
            import["name"] = $"Bulk{targetLogicalName}Import";
            import["modecode"] = new OptionSetValue(0);

            Guid importId = service.Create(import);
            Debug($"Creating import in CRM...DONE");

            Debug($"Adding import file to import...");
            // Create Import File.
            Entity importFile = new Entity("importfile");

            importFile["content"] = csvBuilder.ToString(); // Read contents from disk.
            importFile["name"] = $"Erstellung {targetLogicalName} - {DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss")}";
            importFile["isfirstrowheader"] = true;
            importFile["usesystemmap"] = true;
            importFile["source"] = "OnTheFlyGeneratedByConnectiv.csv";
            importFile["sourceentityname"] = targetLogicalName;
            importFile["targetentityname"] = targetLogicalName;
            importFile["importid"] = new EntityReference("import", importId);
            importFile["enableduplicatedetection"] = enableDuplicateDetection;
            importFile["fielddelimitercode"] = new OptionSetValue(4); // 1:Colon, 2:Comma, 3:Tab, 4:Semicolon
            importFile["datadelimitercode"] = new OptionSetValue(2); // 1:DoubleQuote, 2:None, 3:SingleQuote
            importFile["processcode"] = new OptionSetValue(1);  // 1:Process, 2:Ignore, 3:Internal

            Guid importFileId = service.Create(importFile);
            Debug($"Adding import file to import...DONE");

            Debug($"Parsing data...");
            ParseImportRequest parseImportRequest = new ParseImportRequest()
            {
                ImportId = importId
            };

            // Actually it is possible to wait for this action to be finished, but this cost way too much
            // time and does not make any difference. Same goes for the transformation.
            ParseImportResponse parseImportResponse =
                (ParseImportResponse)service.Execute(parseImportRequest);

            Debug($"Transforming data...");
            TransformImportRequest transformImportRequest = new TransformImportRequest()
            {
                ImportId = importId
            };
            TransformImportResponse transformImportResponse =
                (TransformImportResponse)service.Execute(transformImportRequest);

            // Upload the records.
            Debug($"Import data...");
            ImportRecordsImportRequest importRequest = new ImportRecordsImportRequest()
            {
                ImportId = importId
            };

            ImportRecordsImportResponse importResponse =
                (ImportRecordsImportResponse)service.Execute(importRequest);
            Debug($"Import data...DONE");

            return importResponse.AsyncOperationId;
        }

        #endregion

        #region BulkUpdate
        public static Dictionary<ExecuteMultipleResponseItem, Entity> UpdateMultiple(this IOrganizationService service, IEnumerable<Entity> entitiesToUpdate, Boolean continueOnError, Boolean throwFirstFault = true, Boolean returnResponses = true)
        {
            return service?.UpdateMultiple(entitiesToUpdate, throwFirstFault, new ExecuteMultipleSettings { ContinueOnError = continueOnError, ReturnResponses = returnResponses });
        }

        public static Dictionary<ExecuteMultipleResponseItem, Entity> UpdateMultiple(this IOrganizationService service, IEnumerable<Entity> entitiesToUpdate, Boolean throwFirstFault = true, ExecuteMultipleSettings settings = null)
        {
            var requests = new OrganizationRequestCollection();
            requests.AddRange(entitiesToUpdate.Select(v => new UpdateRequest
            {
                Target = v
            }));

            var retVal = (service.Execute<ExecuteMultipleResponse>(new ExecuteMultipleRequest
            {
                Requests = requests,
                Settings = settings ?? new ExecuteMultipleSettings
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                }
            })?.Responses ?? new ExecuteMultipleResponseItemCollection()).ToDictionary(v => v, v => v.ToEntity(requests));

            KeyValuePair<ExecuteMultipleResponseItem, Entity> firstFaultedRequest;
            if (throwFirstFault && (firstFaultedRequest = retVal.FirstOrDefault(v => v.Key.Fault != null)).Key != null)
            {
                throw new Exception($"Failed to update {firstFaultedRequest.Value.LogicalName}(id= {firstFaultedRequest.Value.Id}): {firstFaultedRequest.Key.Fault.Message}.{Environment.NewLine}Entity values:{Environment.NewLine}{firstFaultedRequest.Value.ToDetailedString()}");
            }

            return retVal;
        }
        #endregion

        #region Exists

        /// <summary>
        /// Checks if the <paramref name="entity"/>'s id exists. (Using <see cref="QueryExpression"/> to prevent exceptions.)
        /// Uses the <see cref="ConnectivOrgServiceExtensions.OrganizationService"/> to communicate with the CRM.
        /// </summary>
        /// <param name="entity">This record will be checked.</param>
        /// <returns>True if a record having the requested id exists, otherwise false.</returns>
        public static bool Exists(this IOrganizationService service, Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (entity.Id == Guid.Empty) return false;

            var metadata = service.RetrieveMetadata(entity);

            var query = new QueryExpression(entity.LogicalName);
            query.ColumnSet = new ColumnSet(false);
            query.Criteria.AddCondition(metadata.PrimaryIdAttribute, ConditionOperator.Equal, entity.Id);

            return service.RetrieveMultiple(query).Entities.Count > 0;
        }

        #endregion

        #region RetrieveMetadata

        /// <summary>
        /// Retrieves all metadata of the the <paramref name="entity"/>'s table.
        /// </summary>
        /// <param name="entity">Record with logicalname to get the table's metadata from.</param>
        /// <param name="service">CRM Service instance.</param>
        /// <returns>All entity metadata information for the requested table.</returns>
        public static EntityMetadata RetrieveMetadata(this IOrganizationService service, Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (String.IsNullOrWhiteSpace(entity.LogicalName))
            {
                throw new ArgumentException($"Property 'Logicalname' of argument {nameof(entity)} cannot be null.");
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var request = new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entity.LogicalName,
            };

            return ((RetrieveEntityResponse)service.Execute(request)).EntityMetadata;
        }

        #endregion

        public static int GetUserLocale(this IOrganizationService service, Guid userId)
        {
            Entity userSettings = service?.GetAll<Entity>(new QueryExpression("usersettings")
            {
                TopCount = 1,
                ColumnSet = new ColumnSet("uilanguageid"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( "systemuserid", ConditionOperator.Equal, userId )
                    }
                },
                LinkEntities =
                    {
                        new LinkEntity
                        {
                            LinkFromEntityName = "usersettings",
                            LinkFromAttributeName = "systemuserid",
                            LinkToEntityName = "systemuser",
                            LinkToAttributeName = "systemuserid",
                            LinkEntities =
                            {
                                new LinkEntity
                                {
                                    Columns = new ColumnSet("localeid"),
                                    LinkFromEntityName = "systemuser",
                                    LinkFromAttributeName = "organizationid",
                                    LinkToEntityName = "organization",
                                    LinkToAttributeName = "organizationid",
                                    EntityAlias = $"systemuser.organization"
                                }
                            }
                        }
                    }
            }).FirstOrDefault();

            int? localeId = userSettings.GetAttributeValue<int?>("uilanguageid") ?? userSettings.GetAttributeValue<int?>($"systemuser.organization.localeid");
            if (!localeId.HasValue)
            {
                throw new Exception($"Failed to determine userLocale.");
            }

            if (localeId <= 999)
            {
                // https://barrysbeginnersblog.wordpress.com/2018/05/26/dynamics-365-version-9-0-list-of-supported-languages/
                Warning($"Originally obtained localeid: {localeId}. Every id below 1000 seem not to be an accurate localeid. App users seem to have a 0 set as language id. Returning 1033 (english) now, to prevent errors.");
                return 1033;
            }

            return localeId.Value;
        }
    }
}
