using Microsoft.AspNetCore.Mvc;
using CihazYonetim.Models;
using CihazYonetim.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CihazYonetim.Controllers;

[Authorize]
public class CihazController : Controller
{
    private readonly AppDbContext _context;

    public CihazController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var durum = new Durumlar
        {
            Online = await _context.Cihazlar.CountAsync(c => c.Durum == CihazDurumu.Online),
            Offline = await _context.Cihazlar.CountAsync(c => c.Durum == CihazDurumu.Offline),
            Ariza = await _context.Cihazlar.CountAsync(c => c.Durum == CihazDurumu.Ariza),
            Bakim = await _context.Cihazlar.CountAsync(c => c.Durum == CihazDurumu.Bakim),
            CihazSayisi = await _context.Cihazlar.CountAsync(),
            OrtalamaHiz = 45.5
        };

        ViewBag.Online = durum.Online;
        ViewBag.Offline = durum.Offline;
        ViewBag.Ariza = durum.Ariza;
        ViewBag.Bakim = durum.Bakim;
        ViewBag.Cihaz_Sayisi = durum.CihazSayisi;
        ViewBag.Hiz = durum.OrtalamaHiz;
        
        ViewBag.Kullanicilar = await _context.Users.ToListAsync();

        // BAĞLANTI KUSURSUZ: Cihazları, sahipleriyle (User) birlikte getiriyor
        var cihazlar = await _context.Cihazlar.Include(c => c.User).ToListAsync();
        return View(cihazlar);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Ekle(Cihazlar yeniCihaz) 
    {
        try
        {
            if (yeniCihaz != null && !string.IsNullOrEmpty(yeniCihaz.CihazName))
            {
                // 1. Önce cihazı veritabanına kaydet
                _context.Cihazlar.Add(yeniCihaz);
                await _context.SaveChangesAsync();

                // 2. Cihaz eklendiği an Log defterine yazdır!
                var yeniLog = new CihazLog 
                {
                    CihazId = yeniCihaz.Id,
                    Islem = "Yeni Cihaz",
                    Detay = $"{yeniCihaz.CihazName} isimli cihaz ağa dahil edildi.",
                    LogTarihi = DateTime.Now
                };
            
                _context.CihazLoglar.Add(yeniLog);
                await _context.SaveChangesAsync();

                // === BALYOZ: BUMERANG TAKTİĞİ ===
                string geldigiSayfa = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(geldigiSayfa))
                {
                    return Redirect(geldigiSayfa);
                }
            }
        
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PozisyonGuncelle(int cihazId, double x, double y)
    {
        // Cihazı sahibiyle birlikte buluyoruz
        var cihaz = await _context.Cihazlar.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == cihazId);
        
        if (cihaz != null)
        {
            // === GÜVENLİK DUVARI: Başkasının cihazını sürükleyip bırakamasın ===
            bool isAdmin = User.IsInRole("admin");
            bool isOwner = cihaz.User?.Username == User.Identity?.Name;

            if (isAdmin || isOwner)
            {
                cihaz.PositionX = x;
                cihaz.PositionY = y;
                await _context.SaveChangesAsync(); // Pozisyon için loga gerek yok
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Geçit yok! Sadece kendi cihazını taşıyabilirsin." });
        }
        return Json(new { success = false, message = "Cihaz bulunamadı." });
    }

    [HttpPost]
    public async Task<JsonResult> DurumGuncelle(int cihazId, int yeniDurum)
    {
        // Cihazı sahibiyle birlikte buluyoruz
        var cihaz = await _context.Cihazlar.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == cihazId);
        if (cihaz == null) return Json(new { success = false, message = "Cihaz bulunamadı!" });

        // === GÜVENLİK DUVARI: Başkasının şalterini indiremesin ===
        bool isAdmin = User.IsInRole("admin");
        bool isOwner = cihaz.User?.Username == User.Identity?.Name;

        if (!isAdmin && !isOwner)
        {
            return Json(new { success = false, message = "Geçit yok! Sadece kendi zimmetindeki cihazın durumunu değiştirebilirsin." });
        }

        // 1. Cihazın kendi durumunu güncelliyoruz
        cihaz.Durum = (CihazDurumu)yeniDurum;
        _context.Cihazlar.Update(cihaz);

        // 2. GERÇEK LOG KAYDINI VERİTABANINA ÇAKIYORUZ
        var yeniLog = new CihazLog 
        {
            CihazId = cihaz.Id,
            Islem = "Durum Değişikliği",
            Detay = $"{cihaz.CihazName} durumu {cihaz.Durum} olarak güncellendi.",
            LogTarihi = DateTime.Now
        };

        _context.CihazLoglar.Add(yeniLog);

        // İkisini birden veritabanına mühürle
        await _context.SaveChangesAsync();

        return Json(new { success = true, guncelDurum = cihaz.Durum.ToString() });
    }
}