using BoilerContollerApplication.Application;
using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Domain.Enums;

namespace BoilerContollerApplication.Presentation;

/// <summary>
/// Handles console operations
/// </summary>
public class ConsoleUI
{
    public event EventHandler<LogData>? EventHandler;
    private SwitchService _switchService;
    private EventLogger _eventService;
    private BoilerSequenceService _boilerSequenceService;
    private Task? _boilerSequenceBackground = null;
    private readonly object _lock = new object();
    public ConsoleUI(SwitchService switchService, EventLogger eventSerive, BoilerSequenceService boilerSequenceService)
    {
        _switchService = switchService;
        _eventService = eventSerive;
        _boilerSequenceService = boilerSequenceService;
        _switchService.ToggleSwitchEventHandler += Display;
        _switchService.ToggleSwitchEventHandler += _eventService.HandleLog;

        EventHandler += Display;
        EventHandler += _eventService.HandleLog;

        _boilerSequenceService.HandleCycle += Display;
        _boilerSequenceService.HandleCycle += _eventService.HandleLog;

        _boilerSequenceService.CountDownTimer += DisplayCountdown;
    }

    private void DisplayCountdown(object? sender, EventDTO e)
    {
        lock (_lock)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.Write($"status : {e.Status}, Count down : {e.TimeLeft} s remaining");
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(left, top);
        }
    }

    /// <summary>
    /// Start the application
    /// </summary>
    public async Task Start()
    {
        Console.WriteLine("Boiler Controller Initialized");
        await _switchService.PutInSystemLockOutState();
        EventHandler?.Invoke(this, new LogData(DateTime.Now, "Boiler Initialized", "State = Lockout"));
        if (await _switchService.GetInterLockSwitchState() == SwitchState.Open)
        {
            await _switchService.InterLockSwitchOperation(SwitchState.Close);
        }
        if (await _switchService.LockoutReset())
        {
            await this.Menu();
        }
    }

    private async Task Menu()
    {
        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            Console.SetCursorPosition(18, 18);
            string choice = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(choice, out int menuOption))
            {
                Console.WriteLine("Invalid Input!");
                return;
            }

            switch ((MenuOperations)menuOption)
            {
                case MenuOperations.StartSequence:
                    if (_boilerSequenceBackground != null && !_boilerSequenceBackground.IsCompleted)
                    {
                        Console.WriteLine("Cant start sequence! other sequence is running");
                    }
                    else
                    {
                        _boilerSequenceBackground = _boilerSequenceService.StartSequence();
                    }
                    break;
                case MenuOperations.StopSequence:
                    await _boilerSequenceService.StopSequence();
                    break;
                case MenuOperations.SimulateError:
                    await HandleError();
                    break;
                case MenuOperations.ToggleRunInterlockSwitch:
                    var toggleSwitch = (_switchService.InterLockSwitch == SwitchState.Open) ? SwitchState.Close : SwitchState.Open;
                    await _switchService.InterLockSwitchOperation(toggleSwitch);
                    Console.WriteLine($"Inter lock toggled to {toggleSwitch} , Reset if closed");
                    break;
                case MenuOperations.ResetLockout:
                    if (await _switchService.LockoutReset())
                    {
                        Console.WriteLine("Switch is Reseted");
                    }
                    break;
                case MenuOperations.ViewLog:
                    await DisplayLog();
                    break;
                case MenuOperations.Exit:
                    return;
                default:
                    Console.WriteLine("Invalid Input!");
                    break;
            }
        }
    }

    private async Task HandleError()
    {
        try
        {
            if (await _boilerSequenceService.SimulateError())
            {
                Console.WriteLine("In Operational state");
            }
        }
        catch (SystemCrashException ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
    }

    private async Task DisplayLog()
    {
        var log = _eventService.GetLogData();
        foreach (var item in await log)
        {
            Console.WriteLine($" {item.TimeStamp}  |  {item.Event}  |  {item.EventData} ");
        }
    }

    private void Display(object? sender, LogData data)
    {
        int left = Console.CursorLeft;
        int top = Console.CursorTop;
        Console.SetCursorPosition(0, 5);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"[Event] : {data.Event}");
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(left, top);
    }

    private void DisplayMenu()
    {
        int left = Console.CursorLeft;
        int top = Console.CursorTop;
        Console.SetCursorPosition(0, 10);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 10);
        Console.Write("Boiler Menu\n1. Start Sequence\n2. Stop Sequence\n3. Simulate Error\n" +
    "4. Toggle Run Interlock Switch\n5. Reset Lockout\n6. View Log\n7.Exit\nEnter your choice: ");
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(left, top);
    }
}
