using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_commerce_Mobile.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip authentication check for Auth controller
            if (context.Controller.GetType().Name == "AuthController")
            {
                base.OnActionExecuting(context);
                return;
            }

            // Check if user is logged in
            if (HttpContext.Session.GetString("UserId") == null)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
