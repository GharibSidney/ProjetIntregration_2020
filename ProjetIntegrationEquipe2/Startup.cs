using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjetIntegrationEquipe2.Startup))]
namespace ProjetIntegrationEquipe2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
