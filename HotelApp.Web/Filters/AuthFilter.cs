using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelApp.Web.Filters;
public class AuthFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Session.IsAuthenticated())
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
