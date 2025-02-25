using Dan.Plugin.Nav.Models;

namespace Dan.Plugin.Nav.Mappers;

public interface IEmploymentHistoryMapper
{
    EmploymentHistory Map(NavEmployeeQueryResponse input);
}

public class EmploymentHistoryMapper : IEmploymentHistoryMapper
{
    public EmploymentHistory Map(NavEmployeeQueryResponse input)
    {
        return new EmploymentHistory()
        {
            EmploymentInfo = input?.Data?.EmploymentInfoResponse?.EmploymentInfo
        };
    }
}
