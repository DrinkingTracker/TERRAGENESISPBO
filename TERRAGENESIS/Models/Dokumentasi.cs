using System;

namespace TERRAGENESIS.Models
{
    /// <summary>Foto penanaman yang diunggah admin untuk satu donasi.</summary>
    public sealed class Dokumentasi
    {
        public Dokumentasi(int id, int donasiId, int diunggahOleh, string pathFoto, DateTime diunggahPada)
        {
            Id = id;
            DonasiId = donasiId;
            DiunggahOleh = diunggahOleh;
            PathFoto = pathFoto;
            DiunggahPada = diunggahPada;
        }

        public int Id { get; }
        public int DonasiId { get; }
        public int DiunggahOleh { get; }
        public string PathFoto { get; }
        public DateTime DiunggahPada { get; }
    }
}
