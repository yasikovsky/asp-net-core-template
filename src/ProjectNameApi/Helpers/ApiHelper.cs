using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectNameApi.Config;
using ProjectNameApi.Enums;

namespace ProjectNameApi.Helpers
{
    public class ApiHelper
    {
        public static int DefaultRowLimit = 30;
        
        public static EnvironmentType Environment;

        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {new JsonStringEnumConverter(), new JsonDateTimeConverter()}
        };
        
        public static string RandomString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static string AddSqlCondition(string existingCondition, string newCondition)
        {
            return string.IsNullOrEmpty(existingCondition) ? newCondition : $"{existingCondition} AND {newCondition}";
        }
        
        public static string GetMimeTypeFromFilename(string fileNameOrPath)
        {
            var extension = Path.GetExtension(fileNameOrPath);

            return extension.ToLower() switch
            {
                ".apng" => "image/apng",
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".ico" or ".cur" => "image/x-icon",
                ".jpg" or ".jpeg" or ".jfif" or ".pjpeg" or ".pjp" => "image/jpeg",
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".svg" => "image/svg+xml",
                ".tif" or ".tiff" => "image/tiff",
                ".webp" => "image/webp",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                _ => "image/jpeg"
            };
        }
        
        public static string GetUniqueFilename(string inputFilename)
        {
            var directory = Path.GetDirectoryName(inputFilename);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputFilename);
            var extension = Path.GetExtension(inputFilename);
            var filenameSuffix = 0;
            var filename = $"{directory}{Path.DirectorySeparatorChar}{fileNameWithoutExt}{extension}";
            var fileExists = File.Exists(filename);

            while (fileExists)
            {
                filename =
                    $"{directory}{Path.DirectorySeparatorChar}{fileNameWithoutExt}-{++filenameSuffix}{extension}";
                fileExists = File.Exists(filename);
            }

            return filename;
        }
    }
}