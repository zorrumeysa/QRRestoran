using System.ComponentModel.DataAnnotations;

namespace QRRestoran.Models
{
    public class Siparis
    {
        public int Id { get; set; }

        [Required]
        public string MasaNo { get; set; } = string.Empty;

        [Required]
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        [Required]
        public decimal ToplamTutar { get; set; }

        // ➕ Ödeme tipi: Nakit / Kart
        public string? OdemeTipi { get; set; } // "Nakit" veya "Kart"

        // ➕ Ödeme yapıldı mı?
        public bool OdemeTamamlandi { get; set; } = false;

        // ➕ Sipariş durumu (isteğe bağlı)
        public string? SiparisDurumu { get; set; } = "Hazırlanıyor";

        // ➕ Sipariş detayları
        public ICollection<SiparisDetay>? Detaylar { get; set; }
    }
}
