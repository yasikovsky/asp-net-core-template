using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using ProjectNameApi.Entities;
using ProjectNameApi.Entities.Users;
using ProjectNameApi.Enums;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace ProjectNameApi.Services
{
    public class DbGateway : IDisposable
    {
        public IDbConnection Connection;

        public DbGateway()
        {
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public DbGateway(string connectionString) : this()
        {
            AddConnection(connectionString);
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }

        /// <summary>
        ///     Establish connection to DB and set up ORM libraries
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public void AddConnection(string connectionString)
        {
            // Connection init
            Connection = new NpgsqlConnection(connectionString);

            // SimpleCRUD config setting
            var resolver = new DbResolver();
            SimpleCRUD.SetTableNameResolver(resolver);
            SimpleCRUD.SetColumnNameResolver(resolver);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
        }

        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="id">ID of the object to get</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<T> GetAsync<T>(Guid id)
        {
            return await Connection.GetAsync<T>(id);
        }
        
        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="id">ID of the object to get</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<T> GetAsync<T>(Guid? id)
        {
            return await Connection.GetAsync<T>(id);
        }

        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Anonymous object with conditions (e.g. new {Color = "Blue"})</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<T> GetAsync<T>(object conditions)
        {
            var ienum = await ListAsync<T>(conditions);

            if (ienum != null && ienum.Count > 0)
                return ienum[0];

            return default;
        }

        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Complete SQL WHERE condition (i.e. WHERE Color = 'Blue')</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<T> GetAsync<T>(string conditions)
        {
            var ienum = await ListAsync<T>(conditions);

            if (ienum != null && ienum.Count > 0)
                return ienum[0];

            return default;
        }

        /// <summary>
        ///     Gets a list of objects of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <returns>A list of entities of type T</returns>
        public async Task<List<T>> ListAsync<T>()
        {
            var ienum = await Connection.GetListAsync<T>();

            return ienum.AsList();
        }

        /// <summary>
        ///     Gets a list of objects of type T from database asynchronously based on specified conditions
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Anonymous object with conditions (e.g. new {Color = "Blue"})</param>
        /// <returns>A list of entities of type T that satisfy given conditions</returns>
        public async Task<List<T>> ListAsync<T>(object conditions)
        {
            var ienum = await Connection.GetListAsync<T>(conditions);

            return ienum.AsList();
        }

        /// <summary>
        ///     Gets a list of objects of type T from database asynchronously based on specified conditions
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Complete SQL WHERE condition (i.e. WHERE Color = 'Blue')</param>
        /// <returns>A list of entities of type T that satisfy given conditions</returns>
        public async Task<List<T>> ListAsync<T>(string conditions)
        {
            var ienum = await Connection.GetListAsync<T>(conditions);

            return ienum.AsList();
        }

        /// <summary>
        ///     Gets a list of objects of type T from database asynchronously based on specified conditions
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Complete SQL WHERE condition (i.e. WHERE Color = 'Blue')</param>
        /// <param name="parameters">An Dapper object with SQL parameters</param>
        /// <returns>A list of entities of type T that satisfy given conditions</returns>
        public async Task<List<T>> ListAsync<T>(string conditions, object parameters)
        {
            var ienum = await Connection.GetListAsync<T>(conditions, parameters);

            return ienum.AsList();
        }

        /// <summary>
        ///     Gets a paged list of objects of type T from database asynchronously based on specified conditions
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="pageNumber">Number of page to start returning records from</param>
        /// <param name="rowsPerPage">Rows to display per page (for calculating proper offset)</param>
        /// <param name="conditions">Optional full SQL WHERE clause (e.g. WHERE Color = 'Blue')</param>
        /// <param name="orderBy">Optional content of SQL ORDER BY clause (e.g. "Color ASC")</param>
        /// <returns>A slice of a list of entities of type T that satisfy given conditions</returns>
        public async Task<List<T>> ListPagedAsync<T>(int pageNumber, int rowsPerPage, string conditions,
            string orderBy = "")
        {
            var ienum = await Connection.GetListPagedAsync<T>(pageNumber, rowsPerPage, conditions, orderBy);

            return ienum.AsList();
        }

        public async Task<int> CountAsync<T>(string conditions = "")
        {
            var result = await Connection.RecordCountAsync<T>(conditions);

            return result;
        }

        /// <summary>
        ///     Gets a paged list of objects of type T from database asynchronously based on specified conditions
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="pageNumber">Number of page to start returning records from</param>
        /// <param name="rowsPerPage">Rows to display per page (for calculating proper offset)</param>
        /// <param name="conditions">Optional anonymous object with conditions (e.g. new {Color = "Blue"})</param>
        /// <param name="orderBy">Optional content of SQL ORDER BY clause (e.g. "Color ASC")</param>
        /// <returns>A slice of a list of entities of type T that satisfy given conditions</returns>
        public async Task<List<T>> ListPagedAsync<T>(int pageNumber, int rowsPerPage, object conditions = null,
            string orderBy = "")
        {
            var ienum = await Connection.GetListPagedAsync<T>(pageNumber, rowsPerPage, "", orderBy, conditions);

            return ienum.AsList();
        }

        /// <summary>
        ///     Inserts an entity to database
        /// </summary>
        /// <typeparam name="T">Inserted object type</typeparam>
        /// <param name="item">Inserted object</param>
        /// <returns>Identity of inserted type if it has a primary key, otherwise null</returns>
        public async Task<Guid> InsertAsync<T>(T item)
        {
            return await Connection.InsertAsync<Guid, T>(item);
        }

        public async Task InsertBulkAsync<T>(List<T> items)
        {
            Connection.Open();
            using var transaction = Connection.BeginTransaction();

            foreach (var item in items) 
                await Connection.InsertAsync<Guid, T>(item);

            transaction.Commit();
            Connection.Close();
        }
        
        public async Task InsertBulkWithChangeLogAsync<T>(List<T> items, EntityType entityType, User user, Guid? parentObjectId = null)
        {
            Connection.Open();
            using var transaction = Connection.BeginTransaction();

            foreach (var item in items)
                await InsertWithChangelogAsync(item, entityType, user, parentObjectId);

            transaction.Commit();
            Connection.Close();
        }

        /// <summary>
        ///     Updates an entity in the database
        /// </summary>
        /// <typeparam name="T">Updated object type</typeparam>
        /// <param name="item">Updated item (a record with the same ID must already be existing in DB)</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int?> UpdateAsync<T>(T item)
        {
            return await Connection.UpdateAsync(item);
        }

        public async Task<Guid> InsertWithChangelogAsync<T>(T currentState, EntityType entityType, User user, Guid? parentObjectId = null)
        {
            var newId = await InsertAsync(currentState);
            currentState = SetKeyValue(currentState, newId);
            
            var changeLog = ChangeLog.GetChangeLog(currentState, default, entityType.ToString(), GetKeyValue(currentState), user, parentObjectId);

            await InsertAsync(changeLog);

            return newId;
        }
        
        public async Task<int?> UpdateWithChangelogAsync<T>(T currentState, T prevState, EntityType entityType, User user, Guid? parentObjectId = null)
        {
            var changeLog = ChangeLog.GetChangeLog(currentState, prevState, entityType.ToString(), GetKeyValue(currentState), user, parentObjectId);

            if (changeLog.ChangeLogStatus != ChangeLogStatus.NoChange)
            {
                await InsertAsync(changeLog);
            }

            return await Connection.UpdateAsync(currentState);
        }
        
        public async Task<int?> DeleteWithChangelogAsync<T>(T prevState, EntityType entityType, User user, Guid? parentObjectId = null)
        {
            var changeLog = ChangeLog.GetChangeLog(default, prevState, entityType.ToString(), GetKeyValue(prevState), user, parentObjectId);

            await InsertAsync(changeLog);

            return await DeleteAsync(prevState);
        }

        /// <summary>
        ///     Deletes an entity from the database
        /// </summary>
        /// <typeparam name="T">Deleted object type</typeparam>
        /// <param name="item">Item to be deleted</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int?> DeleteAsync<T>(T item)
        {
            return await Connection.DeleteAsync(item);
        }

        /// <summary>
        ///     Deletes a list of entities from database
        /// </summary>
        /// <typeparam name="T">Deleted object type</typeparam>
        /// <param name="conditions">Anonymous object with conditions (e.g. new {Color = "Blue"})</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int?> DeleteListAsync<T>(object conditions)
        {
            return await Connection.DeleteListAsync<T>(conditions);
        }

        /// <summary>
        ///     Deletes a list of entities from database
        /// </summary>
        /// <typeparam name="T">Deleted object type</typeparam>
        /// <param name="conditions">Full SQL WHERE clause (e.g. WHERE Color = 'Blue')</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int?> DeleteListAsync<T>(string conditions = "")
        {
            return await Connection.DeleteListAsync<T>(conditions);
        }

        /// <summary>
        ///     Deletes an entity from the database
        /// </summary>
        /// <typeparam name="T">Deleted object type</typeparam>
        /// <param name="id">Id of the item to be deleted</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int?> DeleteAsync<T>(Guid id)
        {
            return await Connection.DeleteAsync<T>(id);
        }

        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Anonymous object with conditions (e.g. new {Color = "Blue"})</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<bool> ExistsAsync<T>(object conditions)
        {
            var result = await Connection.GetListAsync<T>(conditions);

            return result.Any();
        }

        /// <summary>
        ///     Gets the object of type T from database asynchronously
        /// </summary>
        /// <typeparam name="T">Returned object type</typeparam>
        /// <param name="conditions">Complete SQL WHERE condition (i.e. WHERE Color = 'Blue')</param>
        /// <returns>A single entity of type T with single GUID identity</returns>
        public async Task<bool> ExistsAsync<T>(string conditions)
        {
            var result = await Connection.GetListAsync<T>(conditions);

            return result.Any();
        }

        public static PropertyInfo GetKey<T>(T obj)
        {
            var type = typeof(T);
            return type.GetProperties().First(a => Attribute.IsDefined(a, typeof(Dapper.KeyAttribute)) || Attribute.IsDefined(a, typeof(KeyAttribute)));
        }

        public static Guid GetKeyValue<T>(T obj)
        {
            var key = GetKey(obj);

            var value = key.GetValue(obj, null);
            return value == null ? default : Guid.Parse(value.ToString() ?? string.Empty);
        }
        
        public static T SetKeyValue<T>(T obj, Guid keyValue)
        {
            var key = GetKey(obj);

            key.SetValue(obj, keyValue);

            return obj;
        }
    }
}