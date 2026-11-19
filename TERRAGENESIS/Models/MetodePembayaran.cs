using System;

namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Metode pembayaran (abstraction + polymorphism). Di database hanya ada tabel
    /// <c>metode_pembayaran</c> (id, metode); kelas konkretnya dipilih lewat <see cref="Buat"/>.
    /// </summary>
    public abstract class MetodePembayaran
    {
        protected MetodePembayaran(int id, string metode)
        {
            Id = id;
            Metode = metode;
        }

        public int Id { get; }

        /// <summary>Nilai kolom <c>metode_pembayaran.metode</c>, contoh "Transfer Bank".</summary>
        public string Metode { get; }

        /// <summary>Langkah pembayaran yang tampil di kotak "Instruksi Pembayaran".</summary>
        public abstract string[] AmbilInstruksi(int total);

        protected static string Rupiah(int nilai) => Format.Rupiah(nilai);

        public static MetodePembayaran Buat(int id, string metode)
        {
            return metode switch
            {
                "Transfer Bank" => new TransferBank(id, metode),
                "E-Wallet" => new EWallet(id, metode),
                "QRIS" => new Qris(id, metode),
                _ => throw new ArgumentException($"Metode pembayaran tidak dikenal: {metode}", nameof(metode)),
            };
        }
    }

    public sealed class TransferBank : MetodePembayaran
    {
        public TransferBank(int id, string metode) : base(id, metode)
        {
        }

        public override string[] AmbilInstruksi(int total) =>
        [
            $"Lakukan transfer bank sebesar {Rupiah(total)}",
            "Upload bukti transfer",
        ];
    }

    public sealed class EWallet : MetodePembayaran
    {
        public EWallet(int id, string metode) : base(id, metode)
        {
        }

        public override string[] AmbilInstruksi(int total) =>
        [
            $"Lakukan pembayaran lewat e-wallet sebesar {Rupiah(total)}",
            "Upload tangkapan layar bukti pembayaran",
        ];
    }

    public sealed class Qris : MetodePembayaran
    {
        public Qris(int id, string metode) : base(id, metode)
        {
        }

        public override string[] AmbilInstruksi(int total) =>
        [
            $"Scan kode QRIS lalu bayar sebesar {Rupiah(total)}",
            "Upload bukti pembayaran",
        ];
    }
}
