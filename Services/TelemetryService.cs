using System;
using System.Text.RegularExpressions;
using TelemetryCollectorService.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Services
{
    interface ITelemetryService
    {
        public Task SendMetrics(string sessionId, MachineMetrics metrics);
    }
    class TelemetryService: ITelemetryService
    {
        public async Task SendMetrics(string sessionId, MachineMetrics metrics)
        {
            
        }
    }
}