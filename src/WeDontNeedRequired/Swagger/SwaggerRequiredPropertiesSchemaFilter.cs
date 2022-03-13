using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WeDontNeedRequired.Swagger
{
    internal class SwaggerRequiredPropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties.Count > 0)
            {
                // Tutte le proprietà sono richieste, perciò poniamo l'asterisco di fianco a ciascuna
                schema.Required = schema.Properties
                    // .Where(p => !p.Value.Nullable)
                    .Select(p => p.Key)
                    .ToHashSet();
            }
        }
    }
}