using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CATC.SynAodb.Startup))]
namespace CATC.SynAodb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
