using System.Globalization;
using CsvHelper;
using EduAssignments.Core.Interfaces;

namespace EduAssignments.Services;

public class CsvExportService : IExportService
{
    public void ExportToCsv<T>(IEnumerable<T> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
    }
}