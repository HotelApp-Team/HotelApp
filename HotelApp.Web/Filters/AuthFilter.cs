using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelApp.Web.Filters;

// AuthFilter sınıfının amacı, giriş gerektiren web sayfalarını korumaktır.
// Session bilgisini kontrol eder; kullanıcı giriş yapmamışsa Login sayfasına yönlendirir.
// SOLID: SRP kullanır; sadece kullanıcı giriş kontrolünden sorumludur.

public class AuthFilter : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;

    // IConfiguration JWT imza/issuer/audience parametrelerini IsAuthenticated icine tasimak icin gerekli.
    // Bu olmadan filter sadece token suresine bakardi ve imzasi sahte tokenlari kabul ederdi.
    public AuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Session.IsAuthenticated(_configuration))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
