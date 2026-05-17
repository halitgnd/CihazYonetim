using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CihazYonetim.Data;
using CihazYonetim.Models;

namespace CihazYonetim.Controllers;

public class CihazlarlisteController : Controller
{
    private readonly AppDbContext _context;

    public CihazlarlisteController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var cihazlar = await _context.Cihazlar
            .Include(c => c.User) 
            .ToListAsync();
                                     
        return View(cihazlar);
    }
    [HttpPost]
// DİKKAT: ValidateAntiForgeryToken satırını sildik veya yoruma aldık.
    public async Task<IActionResult> Add(Cihazlar formdanGelen)
    {
        try 
        {
            // 1. Önce Cihazı Kaydetmeyi Deniyoruz
            _context.Cihazlar.Add(formdanGelen);
            await _context.SaveChangesAsync(); 

            // 2. Cihaz eklendiyse Logu Atıyoruz
            var log = new CihazLog
            {
                CihazId = formdanGelen.Id,
                Islem = "Yeni Cihaz",
                Detay = $"{formdanGelen.CihazName} adlı cihaz sisteme eklendi.",
                LogTarihi = DateTime.Now
            };
            _context.CihazLoglar.Add(log);
            await _context.SaveChangesAsync(); 
        
            // BAŞARILIYSA CİHAZLAR LİSTESİNE FIRLAT
            return RedirectToAction("Index", "Cihazlarliste");
        }
        catch (Exception ex)
        {
            // PATLARSA TERMİNALE KUS VE SİMÜLASYON SAYFASINDA KAL
            Console.WriteLine("\n================ SİSTEM KOMPLE PATLADI ================");
            Console.WriteLine("HATA: " + ex.Message);
            if (ex.InnerException != null) 
                Console.WriteLine("İÇ HATA: " + ex.InnerException.Message);
            Console.WriteLine("========================================================\n");
        
            return RedirectToAction("Index", "Cihaz");
        }
    }
      
    // DÜZENLE POST (Formdan Gelen Cihaz Verisini SQL'de Güncelle)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Cihazlar formdanGelen)
    {
        var dbCihaz = await _context.Cihazlar.FindAsync(formdanGelen.Id);

        if (dbCihaz != null)
        {
            var eskiDurum = dbCihaz.Durum;
        
            dbCihaz.CihazName = formdanGelen.CihazName;
            dbCihaz.Durum = formdanGelen.Durum; 
            dbCihaz.UserId = formdanGelen.UserId;

            // EĞER DURUM DEĞİŞTİYSE ÖZEL LOG YAZ
            if (eskiDurum != formdanGelen.Durum)
            {
                string detayMesaj = "";

                // Senin formdaki value değerlerine göre (2: Arıza, 3: Bakım)
                if ((int)formdanGelen.Durum == 2) 
                {
                    detayMesaj = $"{dbCihaz.CihazName} cihazında arıza oluştu.";
                }
                else if ((int)formdanGelen.Durum == 3) 
                {
                    detayMesaj = $"{dbCihaz.CihazName} cihazına bakım gerekiyor.";
                }
                else 
                {
                    // Online veya Offline gibi diğer durumlara geçerse standart log
                    detayMesaj = $"{dbCihaz.CihazName} adlı cihazın durumu '{formdanGelen.Durum}' olarak güncellendi.";
                }

                var log = new CihazLog
                {
                    CihazId = dbCihaz.Id,
                    Islem = "Durum Değişikliği",
                    Detay = detayMesaj,
                    LogTarihi = DateTime.Now
                };
                _context.CihazLoglar.Add(log);
            }
            else
            {
                // Durum değişmediyse sadece ismi falan değiştiyse standart düzenleme logu
                var log = new CihazLog
                {
                    CihazId = dbCihaz.Id,
                    Islem = "Cihaz Güncelleme",
                    Detay = $"{dbCihaz.CihazName} adlı cihazın bilgileri düzenlendi.",
                    LogTarihi = DateTime.Now
                };
                _context.CihazLoglar.Add(log);
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
// SİL POST (Cihazı Veritabanından Kazı)
    // SİL POST (Cihazı Veritabanından Kazı ve Loga Yaz)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // 1. Önce cihazı buluyoruz ki adını hafızaya alalım (Sildikten sonra okuyamayız)
        var cihaz = await _context.Cihazlar.FindAsync(id);
    
        if (cihaz != null)
        {
            string silinenCihazAdi = cihaz.CihazName;

            // 2. Log kaydını hazırlıyoruz
            var log = new CihazLog
            {
                CihazId = null, // Cihaz artık olmadığı için ID'yi null bırakmak en temizi
                Islem = "Cihaz Silme",
                Detay = $"{silinenCihazAdi} adlı cihaz sistemden tamamen kaldırıldı.",
                LogTarihi = DateTime.Now
            };

            // 3. Hem cihazı siliyoruz hem logu ekliyoruz
            _context.Cihazlar.Remove(cihaz);
            _context.CihazLoglar.Add(log);
        
            // 4. Tek bir mühürle (SaveChangesAsync) işlemi SQL'e bitiriyoruz
            await _context.SaveChangesAsync();
        }
    
        return RedirectToAction(nameof(Index));
    }
}