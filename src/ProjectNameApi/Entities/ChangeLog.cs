using System;
using System.Collections.Generic;
using System.Text.Json;
using Dapper;
using ProjectNameApi.Entities.Users;
using ProjectNameApi.Enums;
using ProjectNameApi.Helpers;
using ServiceStack;

namespace ProjectNameApi.Entities
{
    public class ChangeLog
    {
        [Key] public Guid ChangeLogId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid? ParentObjectId { get; set; }
        [JsonColumn]
        public string PreviousState { get; set; }
        [JsonColumn]
        public string CurrentState { get; set; }

        public ChangeLogStatus ChangeLogStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public Guid UserId { get; set; }
        
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string ObjectType { get; set; }

        public static (string current, string previous, ChangeLogStatus status) GetDifference<T>(T current, T prev)
        {
            if (prev == null)
            {
                return (JsonSerializer.Serialize(current, ApiHelper.JsonSerializerOptions), null,
                    ChangeLogStatus.Created);
            }

            if (current == null)
            {
                return (null,
                    JsonSerializer.Serialize(prev, ApiHelper.JsonSerializerOptions), ChangeLogStatus.Deleted);
            }

            var previousDict = new Dictionary<string, string>();
            var currentDict = new Dictionary<string, string>();

            foreach (var prop in prev.GetType().GetProperties())
            {
                var prevValue = prop.GetValue(prev, null) ?? "";
                var currentValue = prop.GetValue(current, null) ?? "";

                if (prevValue.ToString() != currentValue.ToString())
                {
                    previousDict.Add(prop.Name.ToCamelCase(), prop.GetValue(prev, null)?.ToString() ?? "");
                    currentDict.Add(prop.Name.ToCamelCase(), prop.GetValue(current, null)?.ToString() ?? "");
                }
            }
            
            if (previousDict.Count == 0 && currentDict.Count == 0)
                return (null, null, ChangeLogStatus.NoChange);

            return (JsonSerializer.Serialize(currentDict, ApiHelper.JsonSerializerOptions),
                JsonSerializer.Serialize(previousDict, ApiHelper.JsonSerializerOptions), ChangeLogStatus.Updated);
        }

        public static ChangeLog GetChangeLog<T>(T currentState, T previousState, string objectType, Guid objectId,
            User user,
            Guid? parentObjectId)
        {
            var (currState, prevState, status) = GetDifference(currentState, previousState);

            return new ChangeLog
            {
                ObjectId = objectId,
                ChangeDate = DateTime.Now,
                PreviousState = prevState,
                CurrentState = currState,
                ChangeLogStatus = status,
                ParentObjectId = parentObjectId,
                UserId = user.UserId,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                ObjectType = objectType,
            };
        }
    }
}