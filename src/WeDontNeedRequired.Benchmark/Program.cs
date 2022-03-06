using System.Net.Http.Json;
using System.Threading.Tasks.Dataflow;
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
    private TimeTravelConfiguration? model;
    private BufferBlock<HttpRequestMessage>? requestQueue;
    public ActionBlock<HttpRequestMessage>? requestExecutor;
    public int requestsPerIteration = 50000;

    [Params(DeserializationMode.SystemTextJson,
            DeserializationMode.NewtonsoftJson,
            DeserializationMode.NewtonsoftJsonWithRequiredProperties,
            DeserializationMode.NewtonsoftJsonWithRequiredPropertiesAndMissingPropertiesHandling)]
    public DeserializationMode DeserializationMode;

    [Params(8, 12, 16, 24, 32, 48, 64)]
    public int DegreeOfParallelism;

    [GlobalSetup]
    public void GlobalSetup()
    {
        httpClient = CreateClient();
        model = GetTimeTravelConfigurationModel();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        requestQueue = new BufferBlock<HttpRequestMessage>();
        for (int i = 1; i <= requestsPerIteration; i++)
        {
            requestQueue.Post(CreateRequest());
        }

        ExecutionDataflowBlockOptions executionOptions = new (){ MaxDegreeOfParallelism = DegreeOfParallelism };
        requestExecutor = new ActionBlock<HttpRequestMessage>(PerformRequest, executionOptions);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        httpClient!.Dispose();
    }

    [Benchmark]
    public Task MeasureDeserializerPerformance()
    {
        if (requestQueue is null || requestExecutor is null)
        {
            throw new InvalidOperationException("Requests are not available");
        }

        requestQueue.LinkTo(requestExecutor, new DataflowLinkOptions { PropagateCompletion = true });
        requestQueue.Complete();
        return requestExecutor.Completion;
    }

    private async Task PerformRequest(HttpRequestMessage request)
    {
        HttpResponseMessage response = await httpClient!.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CreateRequest()
    {
        return new HttpRequestMessage(HttpMethod.Post, "/TimeTravel/Configure")
        {
            Content = JsonContent.Create(model)
        };
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