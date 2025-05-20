using Microsoft.AspNetCore.Mvc;
using QRRestoran.Data;
using QRRestoran.Models;
using System.Linq;

namespace QRRestoran.Controllers
{
    public class GarsonController : Controller
    {
        private readonly QRRestoranDbContext _context;

        public GarsonController(QRRestoranDbContext context)
        {
            _context = context;
        }

        // 🔔 Garson çağırma işlemi (müşteri tarafı)
        [HttpPost]
        public IActionResult Cagir(string masaNo)
        {
            if (string.IsNullOrEmpty(masaNo))
                return BadRequest("Masa numarası eksik.");

            var cagri = new GarsonCagrisi
            {
                MasaNo = masaNo,
                Tarih = DateTime.Now,
                Durum = "Bekliyor"
            };

            _context.GarsonCagrilari.Add(cagri);
            _context.SaveChanges();

            TempData["GarsonCagri"] = "📣 Garson çağrınız alındı. Birazdan yanınızda olacak.";

            return RedirectToAction("Index", "Menu", new { masaNo });
        }


        // 🔔 Garson çağrılarını listeleme (admin tarafı)
        [HttpGet]
        public IActionResult Cagrilar()
        {
            var liste = _context.GarsonCagrilari
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(liste);
        }
        public IActionResult GarsonCagrilari()
        {
            if (HttpContext.Session.GetString("AdminLogin") != "true")
                return RedirectToAction("Giris");

            var cagriListesi = _context.GarsonCagrilari
                .OrderByDescending(c => c.Tarih)
                .ToList();

            return View(cagriListesi);
        }

    }
}
