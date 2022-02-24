using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using WpfApp1;

namespace Chat_Server
{
    public class ChatHub : Hub
    {
        public Task SendeAnAlle(string text)
        {           
            return Clients.Others.Empfange(text);
        }

        public Task SendMessageToGroup(Message msg)
        {
            return Clients.Group(msg.groupname).GroupEmpfange(msg);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);
        }

        public async Task RemoveGroup(string groupName)
        {
            await Groups.Remove(Context.ConnectionId, groupName);
        }

    }
}
