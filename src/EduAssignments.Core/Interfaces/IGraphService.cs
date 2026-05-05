using EduAssignments.Core.Models;

namespace EduAssignments.Core.Interfaces;

public interface IGraphService
{
    Task<IEnumerable<AssignmentItem>> GetAssignmentsAsync(string? userUpn = null);
}