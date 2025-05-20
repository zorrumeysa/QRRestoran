using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QRRestoran.Models
{
    public class Urun
    {
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; } = "";

        public string? Aciklama { get; set; }

        [Required]
        public decimal Fiyat { get; set; }

        public string? ResimYolu { get; set; }

        public int KategoriId { get; set; }
        public Kategori? Kategori { get; set; }
    }


}
