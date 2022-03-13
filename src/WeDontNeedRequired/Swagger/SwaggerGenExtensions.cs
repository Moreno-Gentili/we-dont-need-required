using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WeDontNeedRequired.Swagger
{
    internal static class SwaggerGenExtensions
    {
        public static void AddSchemaFilters(this SwaggerGenOptions options)
        {
            options.SchemaFilter<SwaggerRequiredPropertiesSchemaFilter>();
        }

        public static void MapTypes(this SwaggerGenOptions options)
        {
            options.MapType(typeof(TimeSpan), () => new OpenApiSchema
            {
                Type = "string",
                Format = "HH:MM:SS",
                Example = new OpenApiString("00:00:00"),
                Nullable = false
            });
        }
    }
}