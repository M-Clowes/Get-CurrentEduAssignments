using EduAssignments.Core.Models;

namespace EduAssignments.Core.Interfaces;

public interface IConfigService
{
    AppConfig GetConfig();
}