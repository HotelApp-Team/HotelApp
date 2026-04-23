using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.Web.Filters;
public class AdminFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 1. Session'dan JWT token'i al
        var token = context.HttpContext.Session.GetString("JWToken");

        // 2. Token yoksa Login sayfasina yonlendir
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // 3. Bozuk bir metin direkt ReadJwtToken'a verilirse hata firlatir ve kullanici
        //    500 sayfasiyla karsilasir. CanReadToken bu kontrolu hatasiz (true/false) yapar.
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            context.HttpContext.Session.Remove("JWToken");
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var jwt = handler.ReadJwtToken(token);

        // 4. Token suresi dolmussa sessionu temizle ve Login sayfasina yonlendir
        if (jwt.ValidTo < DateTime.UtcNow)
        {
            context.HttpContext.Session.Remove("JWToken");
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // 5. Token icindeki role bilgisini oku ve Admin degilse ana sayfaya gonder
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
