using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using CihazYonetim.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC Servislerini ekle
builder.Services.AddControllersWithViews();

// 2. Veritabanı Bağlantısı (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. KİMLİK DOĞRULAMA (AUTHENTICATION) AYARLARI
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";         // Giriş yapmayanları buraya şutla
        options.LogoutPath = "/Auth/Logout";       // Çıkış rotası
        options.AccessDeniedPath = "/Auth/Login"; // Yetkisi yetmeyeni buraya gönder
        options.Cookie.Name = "CihazYonetimAuth";  // Çerezine delikanlı bir isim ver
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // 7 gün boyunca hatırla
    });

var app = builder.Build();

// 4. HTTP İşlem Hattı Yapılandırması (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Statik dosyalar (CSS, JS, Resimler) için şart

// ROTA BULMA
app.UseRouting();

// KİMLİK KONTROLÜ (Sen kimsin?)
app.UseAuthentication(); 

// YETKİ KONTROLÜ (Bu sayfaya girmeye hakkın var mı?)
app.UseAuthorization();

// 5. Rotalama Ayarları
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();