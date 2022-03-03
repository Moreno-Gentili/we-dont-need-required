using System.Text;
using System.Text.Json;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using WeDontNeedRequired.Models;
using WeDontNeedRequired.Serialization;

[assembly: WebApplicationFactoryContentRoot("WeDontNeedRequired, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "E:\\CodicePlastico\\we-dont-need-required\\src\\WeDontNeedRequired\\bin\\Release\\net6.0", "appsettings.json", "1")]

BenchmarkRunner.Run<RequireProperties>();

public class RequireProperties
{
    private HttpClient? httpClient;
    private HttpContent? content;

    [Params(DeserializationMode.SystemTextJson,
            DeserializationMode.NewtonsoftJsonWithRequiredProperties,
            DeserializationMode.NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling)]
    public DeserializationMode DeserializationMode;

    [Params(8, 12, 16, 24, 32, 48, 64)]
    public int DegreeOfParallelism;

    [GlobalSetup]
    public void Setup()
    {
        httpClient = CreateClient();
        TimeTravelConfiguration model = GetTimeTravelConfigurationModel();
        string payload = JsonSerializer.Serialize(model);
        content = new StringContent(payload, Encoding.UTF8, "application/json");
    }

    [Benchmark]
    public async Task Request()
    {
        /*List<Task> tasks = Enumerable.Range(1, 50000).Select(counter => PerformRequest()).ToList();
        await Task.WhenAll(tasks);*/
        for (int i = 0; i < 1152 / DegreeOfParallelism; i++)
        {
            List<Task> tasks = Enumerable.Range(1, DegreeOfParallelism).Select(counter => PerformRequest()).ToList();
            await Task.WhenAll(tasks);
        }
    }

    private async Task PerformRequest()
    {
        HttpResponseMessage response = await httpClient!.PostAsync("/TimeTravel/Configure", content);
        response.EnsureSuccessStatusCode();
    }

    private HttpClient CreateClient()
    {
        return new WebApplicationFactory<WeDontNeedRequired.Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.Sources.Clear();
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "DeserializationMode", DeserializationMode.ToString() },
                        { "Logging:LogLevel:Default", "Error" }
                    });
                });
        }).CreateClient();
    }

    private TimeTravelConfiguration GetTimeTravelConfigurationModel()
    {
        Fixture fixture = new();
        return fixture.Create<TimeTravelConfiguration>();
    }
}