using BoilerContollerApplication.Application;
using BoilerContollerApplication.Infrastructure;
using BoilerContollerApplication.Presentation;

namespace BoilerContollerApplication;

public class Program
{
    private static async Task Main(string[] args)
    {
        var logger = new FileLogger("BoilerLog.csv");
        var switchService = new SwitchService();
        var eventLogger = new EventLogger(logger);
        var consoleUI = new ConsoleUI(switchService, eventLogger);
        await consoleUI.Start();
    }
}
