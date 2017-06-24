using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CATC.FIDS.Startup))]
namespace CATC.FIDS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
