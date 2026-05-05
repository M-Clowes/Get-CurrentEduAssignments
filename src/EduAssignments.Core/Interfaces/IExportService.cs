namespace EduAssignments.Core.Interfaces;

public interface IExportService
{
    void ExportToCsv<T>(IEnumerable<T> data, string filePath);
}