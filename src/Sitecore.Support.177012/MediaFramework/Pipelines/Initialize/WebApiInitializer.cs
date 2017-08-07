using System.Web.Routing;

namespace Sitecore.Support.MediaFramework.Pipelines.Initialize
{
  using System.Web.Http;
  using Sitecore.Pipelines;

  public class WebApiInitializer
  {
    protected void Configure(HttpConfiguration configuration)
    {
      configuration.Routes.MapHttpRoute("Reports", "sitecore/api/mediaframework/reports/{datasource}/{sitename}", defaults: new
      {
        controller = "MediaFrameworkReports",
        action = "Get",
      });

      //if (route.DataTokens != null) route.DataTokens.Add("Namespaces", new[] { "Sitecore.MediaFramework.Analytics.Dashboard" });
    }

    public void Process(PipelineArgs args)
    {
      GlobalConfiguration.Configure(this.Configure);
    }
  }
}
