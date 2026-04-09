using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.Web.Filters;
public class AdminFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Session.GetString("JWToken");

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        //Token'dan role bilgisini oku
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
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
