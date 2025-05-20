using System.ComponentModel.DataAnnotations;

namespace QRRestoran.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciAdi { get; set; } = "";

        [Required]
        public string Sifre { get; set; } = ""; // İsteğe bağlı: hash'li de olabilir
    }

}
