using System;

namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Satu donasi = satu petak, boleh banyak pohon. <see cref="Status"/> hanya bisa diubah lewat
    /// <see cref="Setujui"/>, <see cref="Tolak"/>, <see cref="Tanam"/>, dan <see cref="Selesaikan"/>,
    /// sehingga urutan menunggu → disetujui → ditanam → selesai (atau menunggu → ditolak) selalu terjaga.
    /// </summary>
    public sealed class Donasi
    {
        public const int MaksPanjangNama = 100;
        public const int MaksPanjangPesan = 500;

        public Donasi(int id, int penggunaId, int petakId, int pohonId, string namaDonatur, string? pesan,
            int jumlahPohon, StatusDonasi status, DateTime dibuatPada,
            DateTime? disetujuiPada, DateTime? ditanamPada, DateTime? selesaiPada)
        {
            if (jumlahPohon <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(jumlahPohon), "Jumlah pohon minimal 1.");
            }

            Id = id;
            PenggunaId = penggunaId;
            PetakId = petakId;
            PohonId = pohonId;
            NamaDonatur = namaDonatur;
            Pesan = pesan;
            JumlahPohon = jumlahPohon;
            Status = status;
            DibuatPada = dibuatPada;
            DisetujuiPada = disetujuiPada;
            DitanamPada = ditanamPada;
            SelesaiPada = selesaiPada;
        }

        /// <summary>Donasi baru dari form halaman Donasi; Id diisi database saat disimpan.</summary>
        public static Donasi Baru(int penggunaId, int petakId, int pohonId, string namaDonatur, string? pesan, int jumlahPohon)
        {
            return new Donasi(0, penggunaId, petakId, pohonId, namaDonatur, pesan, jumlahPohon,
                StatusDonasi.Menunggu, DateTime.Now, null, null, null);
        }

        public int Id { get; }
        public int PenggunaId { get; }
        public int PetakId { get; }
        public int PohonId { get; }
        public string NamaDonatur { get; }
        public string? Pesan { get; }
        public int JumlahPohon { get; }

        public StatusDonasi Status { get; private set; }
        public DateTime DibuatPada { get; }
        public DateTime? DisetujuiPada { get; private set; }
        public DateTime? DitanamPada { get; private set; }
        public DateTime? SelesaiPada { get; private set; }

        /// <summary>Donasi aktif memegang petaknya; hanya donasi ditolak yang membebaskan petak.</summary>
        public bool Aktif => Status != StatusDonasi.Ditolak;

        public int HitungTotal(Pohon pohon) => JumlahPohon * pohon.Harga;

        public void Setujui()
        {
            PastikanStatus(StatusDonasi.Menunggu, "disetujui");
            Status = StatusDonasi.Disetujui;
            DisetujuiPada = DateTime.Now;
        }

        public void Tolak()
        {
            PastikanStatus(StatusDonasi.Menunggu, "ditolak");
            Status = StatusDonasi.Ditolak;
        }

        public void Tanam()
        {
            PastikanStatus(StatusDonasi.Disetujui, "ditanam");
            Status = StatusDonasi.Ditanam;
            DitanamPada = DateTime.Now;
        }

        public void Selesaikan()
        {
            PastikanStatus(StatusDonasi.Ditanam, "diselesaikan");
            Status = StatusDonasi.Selesai;
            SelesaiPada = DateTime.Now;
        }

        private void PastikanStatus(StatusDonasi diharapkan, string aksi)
        {
            if (Status != diharapkan)
            {
                throw new InvalidOperationException(
                    $"Donasi berstatus {Status.KeTampilan()} tidak bisa {aksi}; harus {diharapkan.KeTampilan()} dulu.");
            }
        }
    }
}
