using EduAssignments.Core.Interfaces;

namespace EduAssignments.Services;

public class UserService : IUserService
{
    public string FormatUserPrincipalName(string input, string defaultDomain)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        var upn = input.Trim().ToLowerInvariant();

        var domainSuffix = defaultDomain.StartsWith('@') ? defaultDomain : $"@{defaultDomain}";

        if (!upn.EndsWith(domainSuffix))
        {
            upn += domainSuffix;
        }

        return upn;
    }
}