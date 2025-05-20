using System.ComponentModel.DataAnnotations;

namespace QRRestoran.Models
{
    public class GarsonCagrisi
    {
        public int Id { get; set; }

        [Required]
        public string MasaNo { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;

        public string Durum { get; set; } = "Bekliyor"; // veya "Yanıtlandı"
    }
}
