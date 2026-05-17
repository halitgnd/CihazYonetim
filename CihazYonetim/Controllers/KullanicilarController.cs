using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CihazYonetim.Data;
using CihazYonetim.Models;


namespace CihazYonetim.Controllers;

[Authorize(Roles = "admin")] 
public class KullanicilarController : Controller
{
    private readonly AppDbContext _context;

    // Veritabanı bağlantısı
    public KullanicilarController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // BALYOZ BURAYA VURULDU: Eski KullandigiCihaz silindi, yerine Cihazlar listesi bağlandı
        var kullanicilar = await _context.Users
            .Include(u => u.Cihazlar)
            .ToListAsync();
            
        return View(kullanicilar);
    }

    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(User user)
    {
        // 1. Önce kullanıcıyı sisteme kaydediyoruz
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 2. Kullanıcı başarıyla kaydedildikten sonra GENEL LOG tablosuna kaydı çakıyoruz
        var log = new CihazLog
        {
            CihazId = null, // Bu bir kullanıcı kaydı olduğu için cihaz ID'sini boş (null) bırakıyoruz
            Islem = "Yeni Kullanıcı",
            Detay = $"{user.Username} isimli yeni kullanıcı sisteme dahil edildi.",
            LogTarihi = DateTime.Now
        };

        _context.CihazLoglar.Add(log);
        await _context.SaveChangesAsync(); // İşte bu tetiği çekmezsen log falan göremezsin

        return RedirectToAction(nameof(Index));
    }

    // KULLANICI DÜZENLEME METODU
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User formdanGelen)
    {
        var dbUser = await _context.Users.FindAsync(formdanGelen.Id);
        if (dbUser != null)
        {
            string eskiAd = dbUser.Username;
            string eskiRol = dbUser.Role;

            // Bilgileri güncelle
            dbUser.Username = formdanGelen.Username;
            dbUser.Role = formdanGelen.Role;
            if (!string.IsNullOrEmpty(formdanGelen.Password))
            {
                dbUser.Password = formdanGelen.Password; 
            }

            // Logu çakıyoruz
            var log = new CihazLog
            {
                CihazId = null,
                Islem = "Kullanıcı Güncelleme",
                Detay = $"{eskiAd} adlı kullanıcının bilgileri güncellendi. (Yeni Rol: {formdanGelen.Role})",
                LogTarihi = DateTime.Now
            };

            _context.CihazLoglar.Add(log);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            string silinenAd = user.Username;

            // Logu hazırlıyoruz
            var log = new CihazLog
            {
                CihazId = null,
                Islem = "Kullanıcı Silme",
                Detay = $"{silinenAd} adlı kullanıcı sistemden kalıcı olarak silindi.",
                LogTarihi = DateTime.Now
            };

            _context.Users.Remove(user);
            _context.CihazLoglar.Add(log);
        
            await _context.SaveChangesAsync(); // İkisini tek seferde mühürle
        }
        return RedirectToAction(nameof(Index));
    }
}