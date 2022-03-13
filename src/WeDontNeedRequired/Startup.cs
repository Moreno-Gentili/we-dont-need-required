using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WeDontNeedRequired.Serialization;

namespace WeDontNeedRequired;
public class Startup
{
    private readonly IConfiguration configuration;
    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        DeserializationMode deserializationMode = configuration.GetValue<DeserializationMode>("DeserializationMode");

        IMvcBuilder mvcBuilder = services.AddControllers();

        if (deserializationMode is not DeserializationMode.SystemTextJson)
        {
            mvcBuilder.AddNewtonsoftJson(options =>
            {
                if (deserializationMode is DeserializationMode.NewtonsoftJsonWithRequiredProperties or DeserializationMode.NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling)
                {
                    options.SerializerSettings.ContractResolver = new RequirePropertiesContractResolver();
                }
                
                if (deserializationMode is DeserializationMode.NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling)
                {
                    options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                }
            });
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            // opt.SchemaFilter<SwaggerExamplesSchemaFilter>();
            opt.MapType(typeof(TimeSpan), () => new OpenApiSchema
            {
                Type = "string",
                Format = "HH:MM:SS",
                Example = new OpenApiString("00:00:00"),
                Nullable = false
            });
        });
        if (deserializationMode is not DeserializationMode.SystemTextJson)
        {
            services.AddSwaggerGenNewtonsoftSupport();
        }
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger().UseSwaggerUI();
        }
        
        app.UseRouting();

        app.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapControllers();
        });
    }
}
