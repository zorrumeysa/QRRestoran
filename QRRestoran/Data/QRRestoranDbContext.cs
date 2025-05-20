using Microsoft.EntityFrameworkCore;
using QRRestoran.Models;

namespace QRRestoran.Data
{
    public class QRRestoranDbContext : DbContext
    {
        public QRRestoranDbContext(DbContextOptions<QRRestoranDbContext> options) : base(options) { }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<GarsonCagrisi> GarsonCagrilari { get; set; }
        public DbSet<Admin> Adminler { get; set; }

    }
}
