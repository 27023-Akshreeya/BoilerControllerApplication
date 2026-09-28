using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Domain.Enums;

namespace BoilerContollerApplication.Application;

/// <summary>
/// Handles switch service
/// </summary>
public class SwitchService
{
    public SwitchState InterLockSwitch { get; set; } = SwitchState.Open;
    public SwitchState ResetSwitch { get; set; } = SwitchState.Open;
    public event EventHandler<LogData>? ToggleSwitchEventHandler;
    public event Action? InterlockOpened;
    public string SystemStatus { get; set; } = "Lockout";

    /// <summary>
    /// Performs interlock operation
    /// </summary>
    /// <param name="switchState"> the state to switch</param>
    /// <returns>false if invalid, true other wise</returns>
    public async Task<bool> InterLockSwitchOperation(SwitchState switchState)
    {
        if (switchState == SwitchState.Invalid)
        {
            return false;
        }
        InterLockSwitch = switchState;
        OnToggleSwitchLogger(new LogData(DateTime.UtcNow, $"Interlock Switch toggled to {switchState}", $"Interlock : {switchState}"));
        if (switchState == SwitchState.Open)
        {
            await PutInSystemLockOutState();
            InterlockOpened?.Invoke();
        }
        return true;
    }

    /// <summary>
    /// Resets the lockout state and updates the system status to Ready if conditions are met.
    /// </summary>
    /// <returns>True if the system status is set to Ready; otherwise, false.</returns>
    public async Task<bool> LockoutReset()
    {

        ResetSwitch = SwitchState.Close;
        OnToggleSwitchLogger(new LogData(DateTime.UtcNow, $"Boiler Status Changed to Ready", $"Reset : close"));
        if (InterLockSwitch == SwitchState.Close && ResetSwitch == SwitchState.Close)
        {
            SystemStatus = "Ready";
            OnToggleSwitchLogger(new LogData(DateTime.UtcNow, "Boiler Status changed to Ready", "SystemStatus: Ready"));
            return true;
        }
        ResetSwitch = SwitchState.Open;
        return false;
    }

    /// <summary>
    /// Asynchronously retrieves the current state of the interlock switch.
    /// </summary>
    /// <returns> task result contains the current SwitchState of the interlock switch.</returns>
    public async Task<SwitchState> GetInterLockSwitchState() => this.InterLockSwitch;

    /// <summary>
    /// Asynchronously retrieves the current state of the reset switch.
    /// </summary>
    /// <returns>The task result contains the current state of the reset switch.</returns>
    public async Task<SwitchState> GetResetSwitchState() => this.ResetSwitch;

    public async Task<bool> IsSystemReady()
    {
        if (InterLockSwitch == SwitchState.Close && ResetSwitch == SwitchState.Close)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the system status to 'Lockout' and opens the reset switch.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PutInSystemLockOutState()
    {
        SystemStatus = "Lockout";
        ResetSwitch = SwitchState.Open;
    }

    private void OnToggleSwitchLogger(LogData data)
    {
        ToggleSwitchEventHandler?.Invoke(this, data);
    }
}
