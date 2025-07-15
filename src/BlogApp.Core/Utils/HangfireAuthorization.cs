using Hangfire.Dashboard;

namespace BlogApp.Core.Utils;

public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}