using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormBasedAuthentication.Startup))]
namespace FormBasedAuthentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
