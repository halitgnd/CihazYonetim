using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CihazYonetim.Data;
namespace CihazYonetim.Controllers;

public class RaporlarController: Controller
{
   private readonly AppDbContext _context;
   
   public RaporlarController(AppDbContext context)
   {
      _context = context;
   }
   
   public async Task<IActionResult> Index()
   {
     var loglar = await _context.CihazLoglar
         .OrderByDescending(x => x.LogTarihi)
         .Take(50)
         .ToListAsync();
     return View(loglar);
   }
   
}