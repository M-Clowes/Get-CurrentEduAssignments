using Microsoft.Graph;
using Azure.Identity;
using EduAssignments.Core.Interfaces;
using EduAssignments.Core.Models;
using Microsoft.Graph.Models;
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
        var classes = new List<EducationClass>();

        var classScope = userUpn != null
            ? await _graphClient.Education.Users[userUpn].Classes.GetAsync()
            : await _graphClient.Education.Classes.GetAsync();
        if (classScope?.Value == null)
            return assignmentList;

        var classIterator = PageIterator<EducationClass, EducationClassCollectionResponse>
            .CreatePageIterator(
                _graphClient,
                classScope,
                eduClass =>
                {
                    classes.Add(eduClass);
                    return true;
                }
            );
        await classIterator.IterateAsync();

        var tasks = classes.Select(cls => PopulatePerClassAssignmentsAsync(cls, assignmentList));
        await Task.WhenAll(tasks);

        return assignmentList;
    }

    private async Task PopulatePerClassAssignmentsAsync(EducationClass eduClass, List<AssignmentItem> assignmentList)
    {
        var assignmentsResponse = await _graphClient
            .Education
            .Classes[eduClass.Id]
            .Assignments
            .GetAsync();

        if (assignmentsResponse?.Value == null)
            return;

        var assignmentIterator = PageIterator<EducationAssignment, EducationAssignmentCollectionResponse>
            .CreatePageIterator(
                _graphClient,
                assignmentsResponse,
                assignment =>
                {
                    if (assignment.DueDateTime != null &&
                        assignment.DueDateTime >= DateTimeOffset.UtcNow)
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

                    return true;
                }
            );
        await assignmentIterator.IterateAsync();
    }
}