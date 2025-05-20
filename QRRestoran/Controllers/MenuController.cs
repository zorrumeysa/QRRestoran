using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRRestoran.Data;

namespace QRRestoran.Controllers
{
    public class MenuController : Controller
    {
        private readonly QRRestoranDbContext _context;

        public MenuController(QRRestoranDbContext context)
        {
            _context = context;
        }

        // QR koddan gelen masa numarasıyla menüyü açar
        public IActionResult Index(string masaNo)
        {
            if (string.IsNullOrEmpty(masaNo))
            {
                TempData["Uyari"] = "Masa numarası eksik.";
                return RedirectToAction("Index", "Menu", new { masaNo = "1" }); // veya Session'dan al
            }

            ViewBag.MasaNo = masaNo;
            HttpContext.Session.SetString("MasaNo", masaNo);

            var kategoriler = _context.Kategoriler
                .Include(k => k.Urunler)
                .OrderBy(k => k.Ad == "Tatlı" ? 0 :
                              k.Ad == "Kahve" ? 1 :
                              k.Ad == "Ana Yemek" ? 2 :
                              k.Ad == "Ara Sıcak" ? 3 : 9)
                .ToList();

            return View(kategoriler);
        }
    }
}
