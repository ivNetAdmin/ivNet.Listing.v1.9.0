
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace ivNet.Listing.Controllers
{
    public class AdminSiteController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        public AdminSiteController(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult UserStories()
        {
            return View("Admin/UserStories/Index");
        }

        [Themed]
        public ActionResult Configuration()
        {
            return View("Admin/Configuration/Index");
        }        
    }
}