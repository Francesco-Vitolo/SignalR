using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;

namespace ChatClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            HubConnection hubConnection = 
                new HubConnection("http://localhost:56871/");

            IHubProxy chatHubProxy =
                hubConnection.CreateHubProxy("ChatHub");

            chatHubProxy.On("Empfange", (text) => Console.WriteLine(text));

            hubConnection.Start().Wait();

            while(true)
            {
                Console.Write("Bitte Text eingeben: ");
                string s = Console.ReadLine();

                chatHubProxy.Invoke("SendeAnAlle", s).Wait();
            }
        }
    }
}
