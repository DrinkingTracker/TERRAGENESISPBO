namespace TERRAGENESIS.Models
{
    public sealed class Wilayah
    {
        public Wilayah(int id, string nama)
        {
            Id = id;
            Nama = nama;
        }

        public int Id { get; }
        public string Nama { get; }
    }
}
