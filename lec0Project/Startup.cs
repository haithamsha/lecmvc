using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(lec0Project.Startup))]
namespace lec0Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
