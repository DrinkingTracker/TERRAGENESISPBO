namespace TERRAGENESIS.Models
{
    /// <summary>Katalog harga per pohon (tabel <c>pohon</c>), bukan satu baris per pohon yang ditanam.</summary>
    public sealed class Pohon
    {
        public Pohon(int id, int harga)
        {
            Id = id;
            Harga = harga;
        }

        public int Id { get; }
        public int Harga { get; }
    }
}
