using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelApp.Web.Filters;

// AdminFilter sınıfının amacı, admin sayfalarına erişim yetkisini kontrol etmektir.
// Session'daki JWT tokenı okur, kullanıcının giriş yapıp yapmadığını ve Admin rolünü kontrol eder.
// SOLID: SRP kullanır; sadece admin erişim kontrolünden sorumludur.

public class AdminFilter : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;

    // IConfiguration JWT imza/issuer/audience dogrulamasini IsAuthenticated/IsInRole icine tasiyabilmek icin.
    // Aksi halde filter sadece payload claim'lerine bakar ve sahte imzali Admin claim'ini kabul ederdi.
    public AdminFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // IsAuthenticated artik imza + issuer + audience + sure kontrolu yapiyor.
        // Gecersizse session temizlenir ve Login'e yonlendirilir.
        if (!context.HttpContext.Session.IsAuthenticated(_configuration))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // IsInRole de IsAuthenticated(configuration) cagirir; signature dogrulanmamis token Admin claim'i tasisa bile kabul edilmez.
        if (!context.HttpContext.Session.IsInRole(_configuration, "Admin"))
        {
            // Sessiz redirect yerine kullaniciya neden Home'a dustugunu kisa bir mesajla bildiriyoruz.
            // Controller.TempData session uzerinden bir sonraki request'te _Layout'taki Error alert'inde gosterilir.
            if (context.Controller is Controller ctrl)
            {
                ctrl.TempData["Error"] = "Bu sayfaya erisim yetkiniz yok.";
            }

            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
