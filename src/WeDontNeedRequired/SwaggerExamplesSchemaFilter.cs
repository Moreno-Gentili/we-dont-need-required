using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WeDontNeedRequired
{
    internal class SwaggerExamplesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties.Count > 0)
            {
                schema.Required = schema.Properties.Select(p => p.Key).ToHashSet();
            }
        }
    }
}