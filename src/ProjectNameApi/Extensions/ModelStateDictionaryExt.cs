using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectNameApi.Entities;
using ProjectNameApi.Entities.Responses;

namespace ProjectNameApi.Extensions
{
    public static class ModelStateDictionaryExt
    {
        public static List<ValidationError> GetValidationErrors(this ModelStateDictionary modelState)
        {
            var errorList = new List<ValidationError>();

            foreach (var field in modelState)
            {
                var error = new ValidationError
                {
                    Field = field.Key,
                    Errors = field.Value.Errors.Select(a => a.ErrorMessage).ToList()
                };

                errorList.Add(error);
            }

            return errorList;
        }
    }
}