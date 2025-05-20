using Microsoft.AspNetCore.Mvc;
using QRRestoran.Models;
using QRRestoran.Data;
using Newtonsoft.Json;

namespace QRRestoran.Controllers
{
    public class SepetController : Controller
    {
        private readonly QRRestoranDbContext _context;

        public SepetController(QRRestoranDbContext context)
        {
            _context = context;
        }

        private List<SepetUrun> GetSepet()
        {
            var json = HttpContext.Session.GetString("Sepet");
            return json != null ? JsonConvert.DeserializeObject<List<SepetUrun>>(json)! : new List<SepetUrun>();
        }

        private void SaveSepet(List<SepetUrun> sepet)
        {
            HttpContext.Session.SetString("Sepet", JsonConvert.SerializeObject(sepet));
        }

        [HttpPost]
        public IActionResult Ekle(int urunId, int adet)
        {
            var urun = _context.Urunler.Find(urunId);
            if (urun == null) return NotFound();

            var sepet = GetSepet();

            var mevcut = sepet.FirstOrDefault(x => x.UrunId == urunId);
            if (mevcut != null)
                mevcut.Adet += adet;
            else
                sepet.Add(new SepetUrun
                {
                    UrunId = urun.Id,
                    UrunAd = urun.Ad,
                    Adet = adet,
                    Fiyat = urun.Fiyat,
                    KategoriAd = urun.Kategori?.Ad
                });

            SaveSepet(sepet);
            return RedirectToAction("Index", "Menu", new { masaNo = TempData["MasaNo"] });
        }

        public IActionResult Liste()
        {
            return View(GetSepet());
        }

        public IActionResult Sil(int urunId)
        {
            var sepet = GetSepet();
            var urun = sepet.FirstOrDefault(x => x.UrunId == urunId);
            if (urun != null)
                sepet.Remove(urun);

            SaveSepet(sepet);
            return RedirectToAction("Liste");
        }

        public IActionResult Bosalt()
        {
            SaveSepet(new List<SepetUrun>());
            return RedirectToAction("Liste");
        }
    }
}
