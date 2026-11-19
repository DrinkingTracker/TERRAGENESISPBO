using System;

namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Akun aplikasi (abstraction + inheritance): tidak bisa dibuat langsung,
    /// harus lewat turunannya, <see cref="Donatur"/> atau <see cref="Admin"/>.
    /// </summary>
    public abstract class Pengguna
    {
        // Encapsulation: hash password tidak pernah dibuka ke luar kelas.
        private readonly string _passwordHash;

        protected Pengguna(int id, string nama, string email, string passwordHash, DateTime dibuatPada)
        {
            Id = id;
            Nama = nama;
            Email = email;
            _passwordHash = passwordHash;
            DibuatPada = dibuatPada;
        }

        public int Id { get; }
        public string Nama { get; }
        public string Email { get; }
        public DateTime DibuatPada { get; }

        /// <summary>Nilai kolom <c>pengguna.peran</c>: "donatur" atau "admin".</summary>
        public abstract string Peran { get; }

        /// <summary>Nama depan untuk sapaan, contoh "Halo, Natasya!".</summary>
        public string NamaDepan => Nama.Split(' ', StringSplitOptions.RemoveEmptyEntries) is { Length: > 0 } bagian
            ? bagian[0]
            : Nama;

        public bool VerifikasiPassword(string password) => PasswordHasher.Cocok(password, _passwordHash);

        /// <summary>Membuat objek turunan yang sesuai dengan nilai kolom <c>peran</c>.</summary>
        public static Pengguna Buat(int id, string nama, string email, string passwordHash, string peran, DateTime dibuatPada)
        {
            return peran switch
            {
                "admin" => new Admin(id, nama, email, passwordHash, dibuatPada),
                "donatur" => new Donatur(id, nama, email, passwordHash, dibuatPada),
                _ => throw new ArgumentException($"Peran tidak dikenal: {peran}", nameof(peran)),
            };
        }
    }

    public sealed class Donatur : Pengguna
    {
        public Donatur(int id, string nama, string email, string passwordHash, DateTime dibuatPada)
            : base(id, nama, email, passwordHash, dibuatPada)
        {
        }

        public override string Peran => "donatur";

        /// <summary>Membuat donasi baru berstatus menunggu untuk petak yang dipilih.</summary>
        public Donasi BuatDonasi(Petak petak, Pohon pohon, string namaDonatur, string? pesan, int jumlahPohon)
        {
            if (!petak.BisaDipilih)
            {
                throw new InvalidOperationException($"Petak #{petak.Nomor} tidak bisa dipilih.");
            }
            return Donasi.Baru(Id, petak.Id, pohon.Id, namaDonatur, pesan, jumlahPohon);
        }
    }

    public sealed class Admin : Pengguna
    {
        public Admin(int id, string nama, string email, string passwordHash, DateTime dibuatPada)
            : base(id, nama, email, passwordHash, dibuatPada)
        {
        }

        public override string Peran => "admin";

        /// <summary>Menyetujui (true) atau menolak (false) donasi yang masih menunggu.</summary>
        public void VerifikasiDonasi(Donasi donasi, bool disetujui)
        {
            if (disetujui)
            {
                donasi.Setujui();
            }
            else
            {
                donasi.Tolak();
            }
        }
    }
}
