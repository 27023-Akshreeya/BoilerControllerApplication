using BoilerContollerApplication.Application;
using BoilerContollerApplication.Domain;
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
        }

        public void Display(object? sender, LogData data)
        {
            Console.WriteLine($"[Event] : {data.Event}");
        }
    }
}
