using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    // Çıkış Yapma Metodu
    [HttpPost]
    [ValidateAntiForgeryToken] // Güvenlik mührü, başkası seni uzaktan çıkaramasın diye
    public async Task<IActionResult> Logout()
    {
        // Tarayıcıdaki kimlik bilgilerini (Cookie) imha ediyoruz
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Çıkış yaptıktan sonra seni Giriş sayfasına (Login) postalar
        // Eğer giriş sayfanın adı farklıysa (mesela Index) orayı düzenle
        return RedirectToAction("Login", "Account");
    }
}