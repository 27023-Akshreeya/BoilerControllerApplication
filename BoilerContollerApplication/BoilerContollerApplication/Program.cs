using BoilerContollerApplication.Application;
using BoilerContollerApplication.Infrastructure;
using BoilerContollerApplication.Presentation;

namespace BoilerContollerApplication;

public class Program
{
    private static async Task Main(string[] args)
    {
        var logger = new FileLogger("BoilerLog.txt");
        var switchService = new SwitchService();
        var eventLogger = new EventLogger(logger);
        var boilerSequence = new BoilerSequenceService(switchService);
        var consoleUI = new ConsoleUI(switchService, eventLogger, boilerSequence);
        await consoleUI.Start();
    }
}
