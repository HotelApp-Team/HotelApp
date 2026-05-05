using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelApp.Web.Filters;

// AuthFilter sınıfının amacı, giriş gerektiren web sayfalarını korumaktır.
// Session bilgisini kontrol eder; kullanıcı giriş yapmamışsa Login sayfasına yönlendirir.
// SOLID: SRP kullanır; sadece kullanıcı giriş kontrolünden sorumludur.

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
