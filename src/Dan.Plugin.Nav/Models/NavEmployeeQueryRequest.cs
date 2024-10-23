using System;
using Newtonsoft.Json;

namespace Dan.Plugin.Nav.Models;

[Serializable]
public class NavEmployeeQueryRequest
{
    [JsonProperty("query")]
    public static string Query =>
        "query($finnArbeidsforholdVariabler: FinnArbeidsforholdVariabler) { finnArbeidsforhold(finnArbeidsforholdVariabler: $finnArbeidsforholdVariabler) " +
        "{ arbeidsforhold { id type { kode beskrivelse } arbeidssted { type ident } ansettelsesperiode { startdato sluttdato } } } }";

    [JsonProperty("variables")]
    public NavEmployeeQueryVariables Variables { get; set; }
}

[Serializable]
public class NavEmployeeQueryVariables
{
    [JsonProperty("finnArbeidsforholdVariabler")]
    public NavEmployeeVariables Variables { get; set; }
}

[Serializable]
public class NavEmployeeVariables
{
    [JsonProperty("arbeidstakerId")]
    public string Id { get; set; }

    [JsonProperty("arbeidsforholdtype")]
    public string[] EmploymentTypes { get; set; }

    [JsonProperty("rapporteringsordning")]
    public string[] Reporting { get; set; }

    [JsonProperty("arbeidsforholdstatus")]
    public string[] Status { get; set; }

    [JsonProperty("ansettelsesdetaljerhistorikk")]
    public bool HistoricalDetails { get; set; }
}
