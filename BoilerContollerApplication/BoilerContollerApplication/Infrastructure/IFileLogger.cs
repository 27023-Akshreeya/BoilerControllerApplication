using BoilerContollerApplication.Domain;

namespace BoilerContollerApplication.Infrastructure
{
    public interface IFileLogger
    {
        Task AddEventLog(LogData logData);
        Task<IEnumerable<LogData>> GetLogger();
    }
}