using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace HotelApp.Web.Filters;
public class AuthFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {

        // 1. Session'dan JWT token'ı al
        var token = context.HttpContext.Session.GetString("JWToken");

        // 2. Token yoksa, kullanıcıyı Login sayfasına yönlendir
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // 3. Token'i oku
        var handler = new JwtSecurityTokenHandler();
        var jwttoken = handler.ReadJwtToken(token);

        //4. Token suresi dolmussa sessionu temizle ve Login sayfasına yönlendir
        if (jwttoken.ValidTo < DateTime.UtcNow)
        {
            context.HttpContext.Session.Remove("JWToken");
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }
        base.OnActionExecuting(context);
    }
}
