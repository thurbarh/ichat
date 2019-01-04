using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iChat.Hubs
{
    public class ichatHub:Hub
    {

        private static readonly Dictionary<string, string> chatMembers = new Dictionary<string, string>();
        public async Task SendMessage(string name,string message)
        {
            if (!chatMembers.ContainsKey(Context.ConnectionId))
            {
                chatMembers.Add(Context.ConnectionId, name);
                await Clients.All.SendAsync("OnManageChatter", name, 1);
            }
            await Clients.AllExcept(Context.ConnectionId).SendAsync("onMessageReceived", name, message);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Client(Context.ConnectionId).SendAsync("OnConnected", chatMembers.Select(k => k.Value).ToArray());
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var name = chatMembers.FirstOrDefault(k => k.Key == Context.ConnectionId);
            if (!name.Equals(default(KeyValuePair<string, string>)))
            {
                await Clients.All.SendAsync("OnManageChatter", name.Value, 0);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
