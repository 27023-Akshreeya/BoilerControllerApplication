using BoilerContollerApplication.Domain;
using BoilerContollerApplication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerContollerApplication.Application
{
    public class EventLogger
    {
        private FileLogger _logger;
        private SwitchService _switchService;
        public EventLogger(FileLogger logger)
        {
            _logger = logger;
        }

        public void HandleLog(object? sender, LogData data)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _logger.AddEventLog(data);
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }
    }
}
