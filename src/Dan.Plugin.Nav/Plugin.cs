using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dan.Common;
using Dan.Common.Exceptions;
using Dan.Common.Interfaces;
using Dan.Common.Models;
using Dan.Common.Util;
using Dan.Plugin.Nav.Clients;
using Dan.Plugin.Nav.Config;
using Dan.Plugin.Nav.Mappers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dan.Plugin.Nav;

public class Plugin(
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory,
    IOptions<Settings> settings,
    INavClient navClient,
    IEmploymentHistoryMapper employmentHistoryMapper,
    IEvidenceSourceMetadata evidenceSourceMetadata)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<Plugin>();
    private readonly HttpClient _client = httpClientFactory.CreateClient(Constants.SafeHttpClient);
    private readonly Settings _settings = settings.Value;

    [Function(PluginConstants.EmploymentHistory)]
    public async Task<HttpResponseData> GetEmploymentHistory(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
        FunctionContext context)
    {
        EvidenceHarvesterRequest evidenceHarvesterRequest;
        try
        {
            evidenceHarvesterRequest = await req.ReadFromJsonAsync<EvidenceHarvesterRequest>();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Exception while attempting to parse request into EvidenceHarvesterRequest: {exceptionType}: {exceptionMessage}",
                e.GetType().Name, e.Message);
            throw new EvidenceSourcePermanentClientException(PluginConstants.ErrorInvalidInput,
                "Unable to parse request", e);
        }

        return await EvidenceSourceResponse.CreateResponse(req,
            () => GetEvidenceValuesAaReg(evidenceHarvesterRequest));
    }

    private async Task<List<EvidenceValue>> GetEvidenceValuesAaReg(
        EvidenceHarvesterRequest evidenceHarvesterRequest)
    {
        if (string.IsNullOrWhiteSpace(evidenceHarvesterRequest?.SubjectParty?.NorwegianSocialSecurityNumber))
        {
            throw new EvidenceSourcePermanentClientException(PluginConstants.ErrorInvalidInput,
                "Request is missing ssn");
        }

        var aaHistory = await navClient.GetAaInformation(evidenceHarvesterRequest);
        var response = employmentHistoryMapper.Map(aaHistory);
        var ecb = new EvidenceBuilder(evidenceSourceMetadata, PluginConstants.EmploymentHistory);
        ecb.AddEvidenceValue("default", response, PluginConstants.SourceName);
        return ecb.GetEvidenceValues();
    }
}
