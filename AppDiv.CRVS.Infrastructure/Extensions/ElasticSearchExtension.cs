
using AppDiv.CRVS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace AppDiv.CRVS.Infrastructure.Extensions;
public static class ElasticSearchExtension
{
    public static void AddElasticSearch(this IServiceCollection services, ConfigurationManager configuration)
    {
        var baseUrl = configuration["ElasticSettings:baseUrl"];
        var index = configuration["ElasticSettings:defaultIndex"];
        var username = configuration["ElasticSettings:userName"];
        var passowrd = configuration["ElasticSettings:password"];
        var certificateFingerprint = configuration["ElasticSettings:CertificateFingerPrint"];


        var settings = new ConnectionSettings(new Uri(baseUrl ?? ""))
                        .PrettyJson()
                        //.CertificateFingerprint(certificateFingerprint)
                        .BasicAuthentication(username, passowrd)
                        .DefaultIndex(index);
        settings.EnableApiVersioningHeader();
        // AddDefaultMappings(settings);
        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);
        CreateIndex(client, index);
    }
    private static void AddDefaultMappings(ConnectionSettings settings)
    {
        // settings.DefaultMappingFor<dynamic>(m => m.Ignore(p => p.FirstNameStr).Ignore(p => p.MiddleNameStr));
    }
    private static void CreateIndex(IElasticClient client, string indexName)
    {
        if (!client.Indices.Exists(indexName).Exists){

        var createIndexResponse = client.Indices.Create(indexName, index => index.Map<SampleModel>(x => x.AutoMap()));
        }
    }
}