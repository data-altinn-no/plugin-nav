using System;
using Newtonsoft.Json;

namespace Dan.Plugin.Nav.Models;

[Serializable]
public class EmploymentInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public EmploymentType EmploymentType { get; set; }

    [JsonProperty("arbeidssted")]
    public Employer Employer { get; set; }

    [JsonProperty("ansettelsesperiode")]
    public EmploymentTenure Tenure { get; set; }
}

[Serializable]
public class EmploymentTenure
{
    [JsonProperty("startdato")]
    public DateTime? StartDate { get; set; }

    [JsonProperty("sluttdato")]
    public DateTime? EndDate { get; set; }
}

[Serializable]
public class Employer
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("ident")]
    public string Id { get; set; }
}

[Serializable]
public class EmploymentType
{
    [JsonProperty("kode")]
    public string Code { get; set; }

    [JsonProperty("beskrivelse")]
    public string Description { get; set; }
}
