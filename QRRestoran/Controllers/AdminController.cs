using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRRestoran.Data;
using QRRestoran.Models;

public class AdminController : Controller
{
    private readonly QRRestoranDbContext _context;

    public AdminController(QRRestoranDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Giris(string kullaniciAdi, string sifre)
    {
        var admin = _context.Adminler.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi && x.Sifre == sifre);
        if (admin != null)
        {
            HttpContext.Session.SetString("AdminLogin", "true");
            return RedirectToAction("Panel");
        }

        ViewBag.Hata = "❌ Kullanıcı adı veya şifre hatalı!";
        return View();
    }

    public IActionResult Panel()
    {
        ViewBag.ToplamSiparis = _context.Siparisler.Count();
        ViewBag.BekleyenCagri = _context.GarsonCagrilari.Count(x => x.Durum == "Bekliyor");
        ViewBag.UrunSayisi = _context.Urunler.Count();
        ViewBag.KategoriSayisi = _context.Kategoriler.Count();

        return View();
    }

    public IActionResult Cikis()
    {
        HttpContext.Session.Remove("AdminLogin");
        return RedirectToAction("Giris");
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
    [HttpPost]
    public IActionResult CagriYanitla(int id)
    {
        var cagri = _context.GarsonCagrilari.FirstOrDefault(x => x.Id == id);
        if (cagri != null)
        {
            cagri.Durum = "Yanıtlandı";
            _context.SaveChanges();
        }
        return RedirectToAction("GarsonCagrilari");
    }
    // Listele
    public IActionResult Siparisler()
    {
        var siparisler = _context.Siparisler
            .OrderByDescending(x => x.SiparisTarihi)
            .ToList();

        return View(siparisler);
    }

    // Durum Güncelle
    [HttpPost]
    public IActionResult DurumGuncelle(int id, string yeniDurum)
    {
        var siparis = _context.Siparisler.FirstOrDefault(x => x.Id == id);
        if (siparis != null)
        {
            siparis.SiparisDurumu = yeniDurum;
            _context.SaveChanges();
        }
        return RedirectToAction("Siparisler");
    }
    // GET: Yeni Ürün Ekle
    public IActionResult UrunEkle()
    {
        var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/urunler");
        var resimler = Directory.GetFiles(klasorYolu)
                                .Select(Path.GetFileName)
                                .ToList();

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "Ad");
        ViewBag.ResimListesi = new SelectList(resimler);

        return View();
    }



    // POST: Ürün Ekleme işlemi
    [HttpPost]
    public IActionResult UrunEkle(Urun urun, IFormFile resim)
    {
        var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == urun.KategoriId);
        if (kategori == null)
            return BadRequest("Kategori bulunamadı");

        string klasorAdi = kategori.Ad.ToLower().Replace(" ", ""); // örn: Tatlılar → tatlılar
        var klasorYolu = Path.Combine("wwwroot", "images", klasorAdi);
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), klasorYolu);

        // 📁 Klasör yoksa oluştur
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        // 📷 Resim varsa işle
        if (resim != null && resim.Length > 0)
        {
            var dosyaAdi = Guid.NewGuid() + Path.GetExtension(resim.FileName);
            var kayitYolu = Path.Combine(fullPath, dosyaAdi);
            using (var stream = new FileStream(kayitYolu, FileMode.Create))
            {
                resim.CopyTo(stream);
            }

            // 🔗 Veritabanına bu yolu kaydet
            urun.ResimYolu = $"/images/{klasorAdi}/{dosyaAdi}";
        }

        _context.Urunler.Add(urun);
        _context.SaveChanges();

        return RedirectToAction("Urunler");
    }



    public IActionResult TumMasalar()
    {
        // Örnek: 1–20 arası masalar
        var masalar = Enumerable.Range(1, 20).ToList();
        return View(masalar);
    }
    // Ürünleri listele
    public IActionResult Urunler()
    {
        var urunler = _context.Urunler.Include(u => u.Kategori).ToList();
        return View(urunler);
    }

    // GET: Ürün Düzenle
    public IActionResult UrunDuzenle(int id)
    {
        var urun = _context.Urunler.Find(id);
        if (urun == null)
            return NotFound();

        ViewBag.Kategoriler = _context.Kategoriler.ToList();
        return View(urun);
    }

    // POST: Ürün Güncelle
    [HttpPost]
    public IActionResult UrunDuzenle(Urun urun)
    {
        if (ModelState.IsValid)
        {
            _context.Urunler.Update(urun);
            _context.SaveChanges();
            return RedirectToAction("Urunler");
        }

        ViewBag.Kategoriler = _context.Kategoriler.ToList();
        return View(urun);
    }

    // Ürün Sil
    public IActionResult UrunSil(int id)
    {
        var urun = _context.Urunler.Find(id);
        if (urun != null)
        {
            _context.Urunler.Remove(urun);
            _context.SaveChanges();
        }

        return RedirectToAction("Urunler");
    }

}
