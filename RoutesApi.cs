
using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;


namespace ivNet.Listing
{
    public class RoutesApi : IHttpRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var rdl = new List<RouteDescriptor>();
            rdl.AddRange(Routes());
            return rdl;
        }

        private IEnumerable<RouteDescriptor> Routes()
        {
            return new[]
            {
                new HttpRouteDescriptor
                {
                    RouteTemplate = "api/listing/{controller}/{id}",
                    Defaults = new
                    {
                        area = "ivNet.Listing",
                        id = System.Web.Http.RouteParameter.Optional
                    }
                },
            };
        }
    }
}