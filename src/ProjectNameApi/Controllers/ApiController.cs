using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProjectNameApi.Entities.Responses;
using ProjectNameApi.Entities.Users;
using ProjectNameApi.Helpers;

namespace ProjectNameApi.Controllers
{
    public class ApiController : Controller
    {
        protected User CurrentUser => GetCurrentUser();

        /// <summary>
        /// Generic OK (200) result override with a "message" response object
        /// </summary>
        /// <param name="message">Message returned in HTTP response body</param>
        /// <returns>OK (200) with a message</returns>
        protected ObjectResult Ok(string message)
        {
            return base.Ok(new MessageResponse(message));
        }

        /// <summary>
        /// Generic Bad Request (400) result override with a "message" response object
        /// </summary>
        /// <param name="message">Message returned in HTTP response body</param>
        /// <returns>Bad Request (400) with a message</returns>
        protected ObjectResult BadRequest(string message)
        {
            return base.BadRequest(new MessageResponse(message));
        }

        /// <summary>
        /// Generic DB get result
        /// </summary>
        /// <param name="obj">Object fetched from DB</param>
        /// <param name="message">Message returned in HTTP response body</param>
        /// <returns>The object or a Bad Request with a message if it's null </returns>
        protected ObjectResult GetResult(object obj, string message = "Object of this ID does not exist")
        {
            if (obj != null)
                return Ok(obj);

            return BadRequest(message);
        }

        /// <summary>
        /// Generic DB update result
        /// </summary>
        /// <param name="rowsUpdated">Count of rows updated by the DB update call</param>
        /// <returns>OK with success message or a Bad Request message no rows were updated</returns>
        protected ObjectResult UpdateResult(int? rowsUpdated)
        {
            if (rowsUpdated <= 0)
                return BadRequest("Failed to update object");

            return Ok("Object updated successfully");
        }

        /// <summary>
        /// Generic DB insert result
        /// </summary>
        /// <param name="result">ID of the inserted entity</param>
        /// <returns>OK with the ID of inserted entity or a Bad Request if the insert failed</returns>
        protected ObjectResult InsertResult(Guid? result)
        {
            if (result == null || result.Value == Guid.Empty)
                return BadRequest("Failed to add object");

            return Ok(result);
        }

        /// <summary>
        /// Generic DB delete result
        /// </summary>
        /// <param name="rowsDeleted">Count of rows updated by the DB update call</param>
        /// <returns>OK with success message or a Bad Request message no rows were deleted</returns>
        protected ObjectResult DeleteResult(int? rowsDeleted)
        {
            if (rowsDeleted != 1)
                return BadRequest("Failed to delete object");

            return Ok("Object deleted successfully");
        }

        /// <summary>
        /// Function that returns a HTTP response with image file based on the input stream 
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="filename">Image filename</param>
        /// <returns>An image file with proper MIME type from stream if it's not null, or a Bad Request
        /// if the stream is null</returns>
        protected IActionResult ImageResult(Stream stream, string filename)
        {
            if (stream == null)
            {
                return NotFound(new {Message = "Requested image was not found"});
            }

            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = filename ?? "image.jpg",
                Inline = true
            }.ToString());

            return File(stream, ApiHelper.GetMimeTypeFromFilename(filename));
        }

        /// <summary>
        /// Gets current user from HttpContext 
        /// </summary>
        /// <returns>User object or null</returns>
        private User GetCurrentUser()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "user");

            if (userClaim == null) return null;

            var user = JsonSerializer.Deserialize<User>(userClaim.Value, ApiHelper.JsonSerializerOptions);

            return user;
        }
    }
}