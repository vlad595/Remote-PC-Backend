using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using TelemetryCollectorService.Core.Models;

namespace Hubs
{
    public class MainHub: Hub
    {
        private readonly ConcurrentDictionary<string, string> _agents = new();
        public async Task RegisterAgent(string sessionId)
        {
            _agents[sessionId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }
        public async Task SendMetrics(string sessionId, MachineMetrics metrics)
        {
            metrics.CalcCpuLoad();
            await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("ReceiveMetrics", metrics);
            Console.WriteLine($"ReceivedMetricsFromAgent: {sessionId}, {metrics.ToString()}");
        }
        public async Task TurnOfPc(string sessionId, string time = "")
        {
            await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("TurnOfPc", time);
        }
        public async Task ExecuteFile(string sessionId, string path)
        {
            await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("ExecuteFile", path);
        }
        public async Task KillProcess(string sessionId, string processId)
        {
            await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("KillProcess", processId);
        }
    }
}
