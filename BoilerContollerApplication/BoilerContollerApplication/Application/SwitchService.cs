using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Domain.Enums;

namespace BoilerContollerApplication.Application;

public class SwitchService
{
    public SwitchState InterLockSwitch { get; set; } = SwitchState.Open;
    public SwitchState ResetSwitch { get; set; } = SwitchState.Open;
    public event EventHandler<LogData>? ToggleSwitchEventHandler;
    public event Action? InterlockOpened;
    public string SystemStatus { get; set; } = "Lockout";

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

    public async Task<SwitchState> GetInterLockSwitchState() => this.InterLockSwitch;

    public async Task<SwitchState> GetResetSwitchState() => this.ResetSwitch;

    public async Task<bool> IsSystemReady()
    {
        if (InterLockSwitch == SwitchState.Close && ResetSwitch == SwitchState.Close)
        {
            return true;
        }
        return false;
    }
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
