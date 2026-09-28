using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Domain.Enums;

namespace BoilerContollerApplication.Application;

public class SwitchService
{
    public SwitchState InterLockSwitch { get; set; } = SwitchState.Open;
    public SwitchState ResetSwitch { get; set; } = SwitchState.Open;
    public event EventHandler<LogData>? ToggleSwitchEventHandler;

    public async Task<bool> InterLockSwitchOperation(SwitchState switchState)
    {
        if (switchState == SwitchState.Invalid)
        {
            return false;
        }
        InterLockSwitch = switchState;
        OnToggleSwitchLogger(new LogData 
        { 
            TimeStamp = DateTime.UtcNow,
            Event = $"Interlock Switch toggled to {switchState}",
            EventData = $"Interlock : {switchState}"
        });
        return true;
    }

    public async Task<bool> LockoutReset()
    {
        ResetSwitch = SwitchState.Close;
        if (await IsSystemReady())
        {
            OnToggleSwitchLogger(new LogData
            {
                TimeStamp = DateTime.UtcNow,
                Event = $"Boiler Status Changed to Ready",
                EventData = $"Reset : close"
            });
            return true;
        }
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
        ResetSwitch = SwitchState.Open;
    }

    private void OnToggleSwitchLogger(LogData data)
    {
        ToggleSwitchEventHandler?.Invoke(this, data);
    }
}
