using Microsoft.AspNetCore.Mvc;
using CihazYonetim.Data;
using CihazYonetim.Models;
using System.Linq;

namespace CihazYonetim.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // 1. KUTUCUKLAR İÇİN SAYILARI ÇEK
        ViewBag.Online = _context.Cihazlar.Count(c => c.Durum == CihazDurumu.Online);
        ViewBag.Offline = _context.Cihazlar.Count(c => c.Durum == CihazDurumu.Offline);
        ViewBag.Ariza = _context.Cihazlar.Count(c => c.Durum == CihazDurumu.Ariza);
        ViewBag.Bakim = _context.Cihazlar.Count(c => c.Durum == CihazDurumu.Bakim);

        // 2. GERÇEK LOGLARI VERİTABANINDAN ÇEK (Son 6 Kayıt)
        // DİKKAT: DbContext içindeki tablonun adı CihazLoglar değilse burayı kendi tablonun adıyla değiştir!
        var sonLoglar = _context.CihazLoglar
            .OrderByDescending(l => l.Id)
            .Take(6)
            .ToList(); // Önce belleğe alıyoruz ki C# işlemleri veritabanını yormasın

        // Ekrana basılacak formata çeviriyoruz
        var logListesi = sonLoglar.Select(l => new {
            Zaman = l.LogTarihi.ToString("HH:mm:ss"),
            Mesaj = l.Detay,
            Tip = LogRenginiBelirle(l.Detay) // Detaydaki kelimeye göre renk seçer
        }).ToList();

        ViewBag.Loglar = logListesi;

        return View();
    }

    // YARDIMCI METOT: Mesajın içindeki kelimeye göre Terminal Etiket Rengini belirler
    private string LogRenginiBelirle(string detay)
    {
        if (string.IsNullOrEmpty(detay)) return "info"; // Varsayılan Mavi

        if (detay.Contains("Online")) return "success"; // Yeşil
        if (detay.Contains("Offline")) return "info";   // Mavi (veya secondary yapabilirsin)
        if (detay.Contains("Arıza")) return "danger";   // Kırmızı
        if (detay.Contains("Bakım")) return "warning";  // Sarı

        return "info"; // Hiçbiri değilse Mavi
    }
}