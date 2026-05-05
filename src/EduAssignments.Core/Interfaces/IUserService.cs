namespace EduAssignments.Core.Interfaces;

public interface IUserService
{
    string FormatUserPrincipalName(string input, string defaultDomain);
}