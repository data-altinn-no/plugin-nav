using System;
using System.Collections.Generic;

namespace Dan.Plugin.Nav.Models;

[Serializable]
public class EmploymentHistory
{
    public List<EmploymentInfo> EmploymentInfo { get; set; }
}
