using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRRestoran.Data;
using QRRestoran.Models;

namespace QRRestoran.Controllers
{
    public class SiparisController : Controller
    {
        private readonly QRRestoranDbContext _context;

        public SiparisController(QRRestoranDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SiparisiTamamla(string masaNo)
        {
            var json = HttpContext.Session.GetString("Sepet");
            if (string.IsNullOrEmpty(json))
            {
                TempData["SiparisDurum"] = "⚠️ Sepetiniz boş.";
                return RedirectToAction("Index", "Menu", new { masaNo });
            }

            var sepet = JsonConvert.DeserializeObject<List<SepetUrun>>(json)!;

            var siparis = new Siparis
            {
                MasaNo = masaNo,
                SiparisTarihi = DateTime.Now,
                ToplamTutar = sepet.Sum(x => x.Fiyat * x.Adet),
                SiparisDurumu = "Hazırlanıyor",
                OdemeTipi = null,
                OdemeTamamlandi = false
            };

            _context.Siparisler.Add(siparis);
            _context.SaveChanges();

            foreach (var item in sepet)
            {
                _context.SiparisDetaylari.Add(new SiparisDetay
                {
                    SiparisId = siparis.Id,
                    UrunId = item.UrunId,
                    Adet = item.Adet
                });
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("Sepet");

            // ✅ Kullanıcıya sadece bilgi ver
            TempData["SiparisDurum"] = "✅ Siparişiniz başarıyla alındı. Hazırlanıyor...";

            return RedirectToAction("Index", "Menu", new { masaNo });
        }
    }
}
