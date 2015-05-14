using Button;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRConfig))]
namespace Button
{
    public class SignalRConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
