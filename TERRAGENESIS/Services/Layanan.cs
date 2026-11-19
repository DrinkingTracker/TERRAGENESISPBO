using System;
using TERRAGENESIS.Repositories;

namespace TERRAGENESIS.Services
{
    /// <summary>
    /// Tempat semua service dirakit sekali. Pages cukup memakai <c>Layanan.Auth</c>,
    /// <c>Layanan.Lahan</c>, dan <c>Layanan.Donasi</c> tanpa tahu repository atau database.
    /// </summary>
    public static class Layanan
    {
        private static readonly Lazy<Rakitan> _rakitan = new(() => new Rakitan());

        public static AuthService Auth => _rakitan.Value.Auth;
        public static LahanService Lahan => _rakitan.Value.Lahan;
        public static DonasiService Donasi => _rakitan.Value.Donasi;

        private sealed class Rakitan
        {
            public Rakitan()
            {
                var database = new DatabaseConnection("localhost", 5432, "TERRAGENESISPBO", "postgres", "123654");
                var db = database.Sumber;
                var pengguna = new PgPenggunaRepository(db);
                var wilayah = new PgWilayahRepository(db);
                var donasi = new PgDonasiRepository(db);
                var referensi = new PgReferensiRepository(db);

                Auth = new AuthService(pengguna);
                Lahan = new LahanService(wilayah, donasi);
                Donasi = new DonasiService(donasi, wilayah, referensi, new PenyimpananFoto());
            }

            public AuthService Auth { get; }
            public LahanService Lahan { get; }
            public DonasiService Donasi { get; }
        }
    }
}
