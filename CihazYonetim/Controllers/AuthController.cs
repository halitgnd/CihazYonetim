using Microsoft.AspNetCore.Mvc;
using CihazYonetim.Models;
using CihazYonetim.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CihazYonetim.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // ==========================
    // KAYIT OLMA (REGISTER)
    // ==========================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User model, string ConfirmPassword)
    {
        // 1. Şifre Tekrar Kontrolü
        if (model.Password != ConfirmPassword)
        {
            ViewBag.Hata = "Girdiğiniz şifreler birbiriyle uyuşmuyor!";
            return View(model);
        }

        // 2. Kullanıcı Adı Çakışma Kontrolü
        bool kullaniciVarMi = _context.Users.Any(u => u.Username == model.Username);
        if (kullaniciVarMi)
        {
            ViewBag.Hata = "Bu kullanıcı adı zaten mevcut. Başka bir tane seç usta.";
            return View(model);
        }

        // 3. Kayıt İşlemi (Varsayılan rol: user)
        model.Role = "user"; 
        _context.Users.Add(model);
        _context.SaveChanges();

        ViewBag.Basarili = "Kaydınız başarıyla oluşturuldu! Şimdi giriş yapabilirsin.";
        return View(); 
    }
    // GİRİŞ YAPMA (LOGIN)
    [HttpGet]
    public IActionResult Login()
    {
        // Eğer zaten giriş yapmışsa direkt Dashboard'a salla
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, bool RememberMe)
    {
        // Veritabanında kullanıcıyı sorgula
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        
        if (user != null)
        {
            // Kimlik Kartı (Claims) Oluşturma
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties 
            {
                IsPersistent = RememberMe, // "Beni Hatırla" seçeneği
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // 7 gün geçerli olsun
            };

            // Tarayıcıya mühürlü Cookie'yi basıyoruz
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Hata = "Kullanıcı adı veya şifre hatalı usta, tekrar dene.";
        return View();
    }
    // ÇIKIŞ YAPMA (LOGOUT)
    [HttpPost] // Navbar'daki form POST gönderdiği için burası POST olmalı
    [ValidateAntiForgeryToken] // Dışarıdan sahte çıkış isteklerini engeller
    public async Task<IActionResult> Logout()
    {
        // Tarayıcıdaki tüm oturum izlerini siler
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Çıkıştan sonra Login sayfasına geri gönderir
        return RedirectToAction("Login", "Auth");
    }
}