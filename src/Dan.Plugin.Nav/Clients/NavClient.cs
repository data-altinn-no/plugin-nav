using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Dan.Common;
using Dan.Common.Exceptions;
using Dan.Common.Models;
using Dan.Plugin.Nav.Config;
using Dan.Plugin.Nav.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dan.Plugin.Nav.Clients;

public interface INavClient
{
    Task<NavEmployeeQueryResponse> GetAaInformation(EvidenceHarvesterRequest req);
}

public class NavClient(
    IHttpClientFactory clientFactory,
    IOptions<Settings> settings,
    ILoggerFactory loggerFactory) : INavClient
{
    private readonly HttpClient _client = clientFactory.CreateClient(Constants.SafeHttpClient);
    private readonly Settings _settings = settings.Value;
    private readonly ILogger<NavClient> _logger = loggerFactory.CreateLogger<NavClient>();

    public async Task<NavEmployeeQueryResponse> GetAaInformation(EvidenceHarvesterRequest req)
    {
        var baseUrl = _settings.NavUrl;
        const string path = "/aareg/v1/arbeidsforhold/graphql";
        var url = $"{baseUrl}{path}";

        var requestBody = new NavEmployeeQueryRequest
        {
            Variables = new NavEmployeeQueryVariables
            {
                Variables = new NavEmployeeVariables
                {
                    Id = req.SubjectParty!.NorwegianSocialSecurityNumber,
                    EmploymentTypes = ["ordinaertArbeidsforhold","maritimtArbeidsforhold"],
                    Reporting = ["A_ORDNINGEN"],
                    Status = ["AKTIV", "AVSLUTTET"],
                    HistoricalDetails = false
                }
            }
        };
        var request = GetRequest(url, HttpMethod.Post, req.MPToken, requestBody);

        var response = await MakeRequest<NavEmployeeQueryResponse>(request);
        return response;
    }

    private static HttpRequestMessage GetRequest<T>(string url, HttpMethod method, string mpToken, T body = null) where T : class
    {
        var uriBuilder = new UriBuilder(url);
        var uri = uriBuilder.ToString();
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", mpToken);

        if (body == null) return request;

        var jsonbody = JsonConvert.SerializeObject(body);
        var content = new StringContent(jsonbody, Encoding.UTF8, "application/json");
        request.Content = content;

        return request;
    }

    private async Task<T> MakeRequest<T>(HttpRequestMessage request)
    {
        HttpResponseMessage result;
        try
        {
            result = await _client.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            throw new EvidenceSourceTransientException(PluginConstants.ErrorUpstreamUnavailble, "Error communicating with upstream source", ex);
        }

        if (!result.IsSuccessStatusCode)
        {
            throw result.StatusCode switch
            {
                HttpStatusCode.NotFound => new EvidenceSourcePermanentClientException(PluginConstants.ErrorNotFound, "Upstream source could not find the requested entity (404)"),
                HttpStatusCode.BadRequest => new EvidenceSourcePermanentClientException(PluginConstants.ErrorInvalidInput,  "Upstream source indicated an invalid request (400)"),
                _ => new EvidenceSourceTransientException(PluginConstants.ErrorUpstreamUnavailble, $"Upstream source retuned an HTTP error code ({(int)result.StatusCode})")
            };
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to parse data returned from upstream source: {exceptionType}: {exceptionMessage}", ex.GetType().Name, ex.Message);
            throw new EvidenceSourcePermanentServerException(PluginConstants.ErrorUnableToParseResponse, "Could not parse the data model returned from upstream source", ex);
        }
    }
}
