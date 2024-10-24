using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Dan.Common;
using Dan.Common.Enums;
using Dan.Common.Interfaces;
using Dan.Common.Models;
using Dan.Plugin.Nav.Config;
using Dan.Plugin.Nav.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NJsonSchema;

namespace Dan.Plugin.Nav;

/// <summary>
/// All plugins must implement IEvidenceSourceMetadata, which describes that datasets returned by this plugin. An example is implemented below.
/// </summary>
public class Metadata : IEvidenceSourceMetadata
{
    private readonly List<string> _belongsToAltinnStudioApps = [PluginConstants.AltinnStudioAppsServiceContext];

    [Function(Constants.EvidenceSourceMetadataFunctionName)]
    public async Task<HttpResponseData> GetMetadataAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
        FunctionContext context)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(GetEvidenceCodes());
        return response;
    }

    public List<EvidenceCode> GetEvidenceCodes()
    {
        return
        [
            new EvidenceCode
            {
                EvidenceCodeName = PluginConstants.EmploymentHistory,
                EvidenceSource = PluginConstants.SourceName,
                ServiceContext = PluginConstants.AltinnStudioAppsServiceContext,
                BelongsToServiceContexts = _belongsToAltinnStudioApps,
                RequiredScopes = "nav:aareg/v1/arbeidsforhold",
                Values =
                [
                    new EvidenceValue
                    {
                        EvidenceValueName = "default",
                        ValueType = EvidenceValueType.JsonSchema,
                        JsonSchemaDefintion = JsonSchema
                            .FromType<EmploymentHistory>()
                            .ToJson(Newtonsoft.Json.Formatting.Indented)
                    }
                ]
            }
        ];
    }
}
