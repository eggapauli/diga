using Owin;
using System.Web.Http;

namespace Diga.Cloud.WebStatus
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "TaskDetail",
                routeTemplate: "{taskKey}",
                defaults: new { controller = "Status", action = "GetTaskById" },
                constraints: new { taskKey = @"\w+" }
            );
            config.Routes.MapHttpRoute(
                name: "TaskSummary",
                routeTemplate: "",
                defaults: new { controller = "Status", action = "GetAllTaskKeys" }
            );

            app.UseWebApi(config);
        }
    }
}
