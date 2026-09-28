using BoilerContollerApplication.Domain;

namespace BoilerContollerApplication.Infrastructure;

public class FileLogger : IFileLogger
{
    private readonly string _filePath;

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    public FileLogger(string filePath)
    {
        _filePath = filePath ?? string.Empty;
        if (!File.Exists(_filePath))
        {
            using (File.Create(_filePath))
            {
            }
        }
    }

    /// <summary>
    /// Adds event information to CSV file
    /// </summary>
    /// <param name="logData"></param>
    /// <returns></returns>
    public async Task AddEventLog(LogData logData)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (new FileInfo(_filePath).Length == 0)
            {
                string header = "TIME STAMP,EVENT,EVENT DATA";
                await File.WriteAllLinesAsync(_filePath, new[] { header });
            }
            string newLog = $"{logData.TimeStamp},{logData.Event},{logData.EventData}";
            await File.AppendAllLinesAsync(_filePath, new string[] { newLog });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets all log information
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<LogData>> GetLogger()
    {
        await _semaphore.WaitAsync();
        try
        {
            var logs = new List<LogData>();
            var allLogs = File.ReadLinesAsync(_filePath);
            bool skipHeader = true;
            await foreach (var log in allLogs)
            {
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }
                var data = log.Split(',', 3);
                if (data.Length == 3)
                {
                    logs.Add(new LogData(DateTime.Parse(data[0]),data[1], data[2]));
                }
            }
            return logs;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
