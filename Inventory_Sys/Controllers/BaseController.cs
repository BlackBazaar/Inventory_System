using System.Web.Mvc;

namespace Inventory_Sys.Controllers
{
    //This controller checks if the user logged in if not it redirects to the login page
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Check if user is not logged in
            if (Session["username"] == null)
            {
                // Redirect to login page
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Login" }
                    });
            }
        }
    }
}
