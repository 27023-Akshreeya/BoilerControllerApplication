using BoilerContollerApplication.Application;
using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerContollerApplication.Presentation
{
    public class ConsoleUI
    {
        public event EventHandler<LogData>? EventHandler;
        private SwitchService _switchService;
        private EventLogger _eventService;
        public ConsoleUI(SwitchService switchService, EventLogger eventSerive)
        {
            _switchService = switchService;
            _eventService = eventSerive;
            _switchService.ToggleSwitchEventHandler += Display;
            _switchService.ToggleSwitchEventHandler += _eventService.HandleLog;
            EventHandler += Display;
            EventHandler += _eventService.HandleLog;
        }

        public async Task Start()
        {
            Console.WriteLine("Boiler Controller Initialized");
            await _switchService.PutInSystemLockOutState();
            EventHandler?.Invoke(this, new LogData
            {
                TimeStamp = DateTime.Now,
                Event = "Boiler Initialized",
                EventData = "State = Lockout"

            });
            if (await _switchService.GetInterLockSwitchState() == SwitchState.Open)
            {
                await _switchService.InterLockSwitchOperation(SwitchState.Close);
            }
            if (await _switchService.LockoutReset())
            {
                this.Menu();
            }
        }

        private void Menu()
        {
            Console.WriteLine("Boiler Menu\n1. Start Sequence\n2. Stop Sequence\n3. Simulate Error\n" +
                "4. Toggle Run Interlock Switch\n5. Reset Lockout\n6. View Log\n7.Exit");

        }

        public void Display(object? sender, LogData data)
        {
            Console.WriteLine($"[Event] : {data.Event}");
        }
    }
}
