using FixItNow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FixItNow.Hubs
{
    public class NotifierHub:Hub
    {
      
        public async Task SendNotification()
        {
            Console.WriteLine("called hub");
            await Clients.All.SendAsync("ReceiveNotification");
        }
    }
}
