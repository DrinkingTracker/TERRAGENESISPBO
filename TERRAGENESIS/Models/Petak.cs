namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Satu kotak di grid 10x10. <see cref="Terisi"/> tidak disimpan di tabel <c>petak</c>,
    /// melainkan dihitung dari ada tidaknya donasi aktif, jadi hanya bisa diisi saat objek dibuat.
    /// </summary>
    public sealed class Petak
    {
        public const int JumlahPerWilayah = 100;

        public Petak(int id, int wilayahId, string namaWilayah, int nomor, bool aktif, bool terisi)
        {
            Id = id;
            WilayahId = wilayahId;
            NamaWilayah = namaWilayah;
            Nomor = nomor;
            Aktif = aktif;
            Terisi = terisi;
        }

        public int Id { get; }
        public int WilayahId { get; }
        public string NamaWilayah { get; }
        public int Nomor { get; }

        /// <summary>Status lahan yang diatur admin (kolom <c>petak.status</c>).</summary>
        public bool Aktif { get; }

        public bool Terisi { get; }

        public bool BisaDipilih => Aktif && !Terisi;

        public string Label => $"Petak #{Nomor} · {NamaWilayah}";
    }
}
