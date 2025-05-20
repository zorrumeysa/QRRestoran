namespace QRRestoran.Models
{
    public class SepetUrun
    {
        public int UrunId { get; set; }
        public string UrunAd { get; set; }
        public int Adet { get; set; }
        public decimal Fiyat { get; set; }
        public string? KategoriAd { get; set; }
    }
}
