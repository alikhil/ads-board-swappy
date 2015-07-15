using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Swappy_V2.Startup))]
namespace Swappy_V2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
