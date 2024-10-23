using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dan.Plugin.Nav.Models;

[Serializable]
public class NavEmployeeQueryResponse
{
    [JsonProperty("data")]
    public Data Data { get; set; }
}

[Serializable]
public class Data
{
    [JsonProperty("finnArbeidsforhold")]
    public EmploymentInfoResponse EmploymentInfoResponse { get; set; }
}

[Serializable]
public class EmploymentInfoResponse
{
    [JsonProperty("arbeidsforhold")]
    public List<EmploymentInfo> EmploymentInfo { get; set; }
}

