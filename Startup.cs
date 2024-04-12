using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Services.Description;

[assembly: OwinStartup(typeof(SuperbrainManagement.Startup))]

namespace SuperbrainManagement
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("ModelDbContext");

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
