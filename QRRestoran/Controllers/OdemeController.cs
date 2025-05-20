using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRRestoran.Data;
using QRRestoran.Models;

namespace QRRestoran.Controllers
{
    public class OdemeController : Controller
    {
        private readonly QRRestoranDbContext _context;

        public OdemeController(QRRestoranDbContext context)
        {
            _context = context;
        }

        public IActionResult Yap(string masaNo)
        {
            if (string.IsNullOrEmpty(masaNo))
            {
                TempData["OdemeDurum"] = "Masa bilgisi alınamadı.";
                return RedirectToAction("Index", "Menu");
            }

            var siparis = _context.Siparisler
                .Include(s => s.Detaylar!)
                    .ThenInclude(d => d.Urun)
                .Where(s => s.MasaNo == masaNo)
                .OrderByDescending(s => s.SiparisTarihi)
                .FirstOrDefault();

            if (siparis == null)
            {
                TempData["OdemeDurum"] = "Bu masa için sipariş bulunamadı.";
                return RedirectToAction("Index", "Menu", new { masaNo });
            }

            return View(siparis);
        }

        [HttpPost]
        public IActionResult YapOdeme(int siparisId, string odemeTipi)
        {
            var siparis = _context.Siparisler.Find(siparisId);
            if (siparis == null)
                return NotFound();

            siparis.OdemeTipi = odemeTipi;
            siparis.OdemeTamamlandi = odemeTipi == "Kart";
            _context.SaveChanges();

            // ✅ Dinamik mesajlar:
            if (odemeTipi == "Nakit")
                TempData["OdemeDurum"] = "💵 Nakit ödeme seçildi. Lütfen kasaya yöneliniz.";
            else
                TempData["OdemeDurum"] = "✅ Ödemeniz başarıyla alındı.";

            return RedirectToAction("Yap", new { masaNo = siparis.MasaNo });
        }

    }
}