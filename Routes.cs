using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace ivNet.Listing
{
    public class Routes : IRouteProvider 
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var rdl = new List<RouteDescriptor>();
            rdl.AddRange(OwnerRoutes());
            rdl.AddRange(AdminRoutes());
            return rdl;
        }

        private IEnumerable<RouteDescriptor> OwnerRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                     Route = new Route(
                        "owner/registration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"},
                            {"controller", "Site"},
                            {"action", "Registration"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                     Route = new Route(
                        "owner/activation/{id}",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"},
                            {"controller", "Site"},
                            {"action", "Activation"},
                            {"id", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"}
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor
                {
                     Route = new Route(
                        "owner/uploadfiles/{id}",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"},
                            {"controller", "SecureSite"},
                            {"action", "UploadFiles"},
                            {"id", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        private IEnumerable<RouteDescriptor> AdminRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                     Route = new Route(
                        "admin/user-stories",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"},
                            {"controller", "AdminSite"},
                            {"action", "UserStories"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"}
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor
                {
                     Route = new Route(
                        "admin/configuration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"},
                            {"controller", "AdminSite"},
                            {"action", "Configuration"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Listing"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}