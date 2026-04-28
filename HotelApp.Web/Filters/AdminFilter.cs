using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.Web.Filters;
public class AdminFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Admin sayfasına girmeden önce kullanıcının gerçekten giriş yapmış olup olmadığını kontrol eder.
        // Token yoksa, bozuksa veya süresi dolmuşsa IsAuthenticated() session bilgilerini temizleyip false döner.
        if (!context.HttpContext.Session.IsAuthenticated())
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // Auth kontrolu IsAuthenticated() icinde yapildi.
        // Burada sadece token icindeki role bilgisini okuyup Admin yetkisini kontrol ediyoruz.
        var token = context.HttpContext.Session.GetString("JWToken")!;
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Token icindeki role bilgisini oku ve Admin degilse ana sayfaya gonder.
        var roles = jwt.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value);

        if (!roles.Contains("Admin"))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }
        base.OnActionExecuting(context);
    }
}
