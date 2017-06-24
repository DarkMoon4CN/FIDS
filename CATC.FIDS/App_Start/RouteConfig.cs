using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CATC.FIDS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "TDeparture",
              url: "TDeparture",
              defaults: new { controller = "Template", action = "Departureflight", id = UrlParameter.Optional },
              namespaces: new string[] { "CATC.FIDS.Controllers" }
             );
            routes.MapRoute(
             name: "Departure",
             url: "Departure",
             defaults: new { controller = "Actual", action = "Departureflight", id = UrlParameter.Optional },
             namespaces: new string[] { "CATC.FIDS.Controllers" }
            );
            routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "EditMain", id = UrlParameter.Optional },
            namespaces: new string[] { "CATC.FIDS.Controllers" }
            );

        }
    }
}
