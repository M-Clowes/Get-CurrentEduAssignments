using Microsoft.Graph;
using Azure.Identity;
using EduAssignments.Core.Interfaces;
using EduAssignments.Core.Models;

namespace EduAssignments.Services;

public class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphClient;

    public GraphService(string tenantId, string clientId, string clientSecret)
    {
        var options = new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };
        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

        _graphClient = new GraphServiceClient(clientSecretCredential);
    }

    public async Task<IEnumerable<AssignmentItem>> GetAssignmentsAsync(string? userUpn = null)
    {
        var assignmentList = new List<AssignmentItem>();

        var classes = userUpn != null
            ? await _graphClient.Education.Users[userUpn].Classes.GetAsync()
            : await _graphClient.Education.Classes.GetAsync();
        if (classes?.Value == null)
            return assignmentList;
        foreach (var eduClass in classes.Value)
        {
            var assignments = await _graphClient.Education.Classes[eduClass.Id].Assignments.GetAsync();

            if (assignments?.Value != null)
            {
                foreach (var assignment in assignments.Value)
                {
                    assignmentList.Add(new AssignmentItem
                    {
                        ClassId = eduClass.Id ?? string.Empty,
                        ClassName = eduClass.DisplayName ?? string.Empty,
                        AssignmentId = assignment.Id ?? string.Empty,
                        DisplayName = assignment.DisplayName ?? string.Empty,
                        Status = assignment.Status?.ToString() ?? string.Empty,
                        DueDateTime = assignment.DueDateTime

                    });
                }
            }
        }
        return assignmentList;
    }
}