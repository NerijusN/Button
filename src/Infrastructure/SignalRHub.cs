using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Button.Infrastructure
{
    [HubName("SignalRHub")]
    public class SignalRHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}