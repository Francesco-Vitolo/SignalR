using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Chat_Server
{
    public class ChatHub : Hub
    {
        public Task SendeAnAlle(string text)
        {           
            return Clients.Others.Empfange(text);
        }

        public Task SendMessageToGroup(string groupName, string message, string name)
        {
            return Clients.Group(groupName).GroupEmpfange($"{name}({Context.ConnectionId.Substring(0,5)})\n{message}\n{DateTime.Now:HH:mm}", groupName, Context.ConnectionId); 
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
