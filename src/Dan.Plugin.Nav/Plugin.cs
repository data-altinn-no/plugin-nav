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
using Newtonsoft.Json;

namespace Dan.Plugin.Nav;

public class Plugin(
    ILoggerFactory loggerFactory,
    INavClient navClient,
    IEmploymentHistoryMapper employmentHistoryMapper,
    IEvidenceSourceMetadata evidenceSourceMetadata)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<Plugin>();

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

        var employmentHistory = await navClient.GetEmploymentHistory(evidenceHarvesterRequest);
        var response = employmentHistoryMapper.Map(employmentHistory);
        var ecb = new EvidenceBuilder(evidenceSourceMetadata, PluginConstants.EmploymentHistory);
        ecb.AddEvidenceValue("default", response, PluginConstants.SourceName);
        return ecb.GetEvidenceValues();
    }

    [Function(PluginConstants.Grunnbeloep)]
    public async Task<HttpResponseData> GetGrunnbeloep(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
        FunctionContext context)
    {
        return await EvidenceSourceResponse.CreateResponse(req,
            () => GetEvidenceValuesGrunnbeloep());
    }

    private async Task<List<EvidenceValue>> GetEvidenceValuesGrunnbeloep()
    {
        var grunnbeloep = await navClient.GetCurrentGAmount();
        
        var ecb = new EvidenceBuilder(evidenceSourceMetadata, PluginConstants.Grunnbeloep);
        ecb.AddEvidenceValue("default", JsonConvert.SerializeObject(grunnbeloep), PluginConstants.SourceName, false);

        return ecb.GetEvidenceValues();
    }
}
