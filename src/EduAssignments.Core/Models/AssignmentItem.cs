namespace EduAssignments.Core.Models;

public class AssignmentItem
{
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set ;} = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? DueDateTime { get; set; }
}