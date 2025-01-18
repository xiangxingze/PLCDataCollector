using Microsoft.AspNetCore.SignalR;
using PLCDataCollector.API.Models;

namespace PLCDataCollector.API.Hubs
{
    public class PLCDataHub : Hub
    {
        public async Task SendData(CollectedData data)
        {
            await Clients.All.SendAsync("ReceiveData", data);
        }

        public async Task SendAlarm(AlarmRecord alarm)
        {
            await Clients.All.SendAsync("ReceiveAlarm", alarm);
        }
    }
} 