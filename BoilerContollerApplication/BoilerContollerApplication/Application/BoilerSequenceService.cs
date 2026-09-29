using BoilerContollerApplication.Domain;

namespace BoilerContollerApplication.Application;
/// <summary>
/// Manages the operational sequence of a boiler system, including startup, shutdown, and error simulation. Provides
/// events for cycle logging and countdown notifications.
/// </summary>
/// <remarks>Handles interlocks via SwitchService and supports cancellation of boiler operations.</remarks>
public class BoilerSequenceService
{
    public event EventHandler<LogData>? HandleCycle;
    public event EventHandler<EventDTO>? CountDownTimer;
    private BoilerSequence boilerSequence = new();
    private CancellationTokenSource source;
    private SwitchService _switchService;
    public BoilerSequenceService(SwitchService switchService)
    {
        this._switchService = switchService;
        this._switchService.InterlockOpened += TriggerCancellation;
    }

    private void TriggerCancellation()
    {
        OnCycleEnds(new LogData(DateTime.UtcNow, "Cancelled", "status: Lockout"));
        source?.Cancel();
    }

    /// <summary>
    /// Initiates the boiler sequence, transitioning through pre-purge, ignition, and operational phases.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StartSequence()
    {
        if (!(await _switchService.IsSystemReady()))
        {
            OnCycleEnds(new LogData(DateTime.UtcNow, "Not Ready", "status: Lockout"));
            return;
        }
        source = new CancellationTokenSource();
        var token = source.Token;
        try
        {
            boilerSequence.Status = "Pre-Purge";
            await RunCountdownPhase("Pre-Purge", 10, token);
            OnCycleEnds(new LogData(DateTime.UtcNow, "Pre-Purge completed", "Status : Pre-Purge"));
            boilerSequence.Status = "Ignition";
            await RunCountdownPhase("Ignition", 10, token);
            OnCycleEnds(new LogData(DateTime.UtcNow, "Ignition phase completed", "Status : Ignition"));
            boilerSequence.Status = "Operational";
            OnCycleEnds(new LogData(DateTime.UtcNow, "Boiler now operational", "status: operational"));
        }
        catch (Exception ex)
        {
            boilerSequence.Status = "Stoped";
            OnCycleEnds(new LogData(DateTime.UtcNow, "Sequence stopped", "status: Lockout"));
        }
    }

    /// <summary>
    /// Keeps countdown
    /// </summary>
    /// <param name="status">boiler status</param>
    /// <param name="seconds">the amount of seconds left</param>
    /// <param name="token">If it had cancelled</param>
    /// <returns></returns>
    private async Task RunCountdownPhase(string status, int seconds, CancellationToken token)
    {
        for (int i = seconds; i > 0; i--)
        {
            token.ThrowIfCancellationRequested();
            CountDownTimer?.Invoke(this, new EventDTO(status, TimeSpan.FromSeconds(i)));
            await Task.Delay(1000);
        }
    }

    private void OnCycleEnds(LogData data)
    {
        HandleCycle?.Invoke(this, data);
    }

    /// <summary>
    /// Stops the boiler sequence, cancels any ongoing operations, and logs the completion status.
    /// </summary>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public async Task StopSequence()
    {
        if (source != null && !source.IsCancellationRequested)
        {
            source.Cancel();
            source.Dispose();
        }
        OnCycleEnds(new LogData(DateTime.UtcNow, "Boiler Sequence Completed", "status : Completed"));
    }

    /// <summary>
    /// Simulates a boiler system error if the current status is operational.
    /// </summary>
    /// <returns>true if the error simulation was initiated; otherwise, false.</returns>
    /// <exception cref="SystemCrashException">Thrown when a boiler system error causes a system crash.</exception>
    public async Task<bool> SimulateError()
    {
        if (!boilerSequence.Status.Equals("Operational"))
        {
            return false;
        }

        await StopSequence();
        throw new SystemCrashException("Boiler system error!");
    }
}
