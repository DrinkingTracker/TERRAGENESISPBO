# Checkpoint — TERRAGENESIS (Proyek PBO, WinUI 3)

Dibuat: 19 November 2026. Diperbarui: 19 November 2026 (setelah revisi ERD 1 dan penyelarasan diagram kelas).
Isi: ringkasan pembahasan sesi pertama.
Unggah file ini bersama `schema.sql` ke sesi berikutnya supaya konteksnya terbawa.

## 1. Tujuan dan lingkungan

- Tujuan: membuat aplikasi OOP (PBO) dengan antarmuka **WinUI 3**.
- Desain Figma: https://www.figma.com/design/HGco7zF20HmR3AXVUVJ1mf/PROJECT-PBO (file key `HGco7zF20HmR3AXVUVJ1mf`)
- Repo GitHub: belum diisi (kolom di pesan pertama kosong).
- Visual Studio 2026 Community, workload **.NET desktop development** dan **WinUI application development** sudah terpasang.
- Proyek sudah dibuat di VS: **TERRAGENESIS (Package)**, konfigurasi Debug x64. Ini pertama kalinya memakai WinUI.
- Database: **PostgreSQL + pgAdmin** sudah terpasang.
- Metodologi: **SDLC waterfall**.

## 2. Konsep aplikasi (dibaca dari Figma)

Platform donasi penanaman pohon. Alur: pilih wilayah di peta → pilih petak kosong di grid 10x10 → isi donasi (jumlah pohon, nama, pesan, metode bayar, upload bukti) → admin verifikasi → penanaman → dokumentasi. Harga Rp 15.000 per pohon.

Layar di Figma: Beranda, Login, Peta wilayah, Lahan (grid + pop-up detail pohon), Donasi, Profile, Donasi Saya, Penanaman Saya. Layar **Register** dan **Admin** belum ada di Figma dan akan ditambahkan.

Catatan dari Figma (hanya struktur yang terbaca, warna/font belum diambil):
- Peta Indonesia berupa vektor per provinsi, bagian tersulit di WinUI. Opsi: mulai dari kartu/tombol wilayah dulu, peta interaktif belakangan (belum diputuskan).
- Nama wilayah tidak seragam: kartu Beranda (Jawa Timur, Kalimantan, Sumatera Utara) vs label peta (Sumatra, Kalimantan, Sulawesi, Papua, Jawa Timur). Daftar final belum diputuskan.
- Ada angka tidak konsisten di desain (panel ringkasan 36 vs 37 petak terisi).
- Halaman Donasi menampilkan Jumlah Pohon 3 / Rp 45.000 dengan satu petak (#23). Ini sesuai keputusan: 1 donasi = 1 petak, banyak pohon.

## 3. Keputusan yang sudah ditetapkan

1. Login sungguhan dengan database, **ada halaman register**.
2. **Layar admin** akan ditambahkan (verifikasi donasi, ubah status, unggah dokumentasi).
3. Verifikasi pembayaran **manual** (upload bukti transfer).
4. **Satu donasi = satu petak, boleh banyak pohon**, tanpa batas pohon per petak.
5. Satu petak hanya bisa dipegang satu donasi aktif; donasi `ditolak` membebaskan petak.
6. Alur status donasi: menunggu → diterima → ditanam → selesai, atau menunggu → ditolak.
7. Password disimpan sebagai hash, query database memakai parameter.
8. Akun admin dibuat manual di database (tidak lewat register).
9. Penyimpanan: PostgreSQL via Npgsql (rencana; versi paket belum dicek).
10. **Revisi ERD 1:** `pohon`, `pembayaran`, dan `metode_pembayaran` menjadi tabel sendiri; tabel `pohon` hanya berisi `id` dan `harga` (diperlakukan sebagai katalog harga, bukan satu baris per pohon yang ditanam).
11. Kolom `kode` di `wilayah` dihapus; kolom nama di `metode_pembayaran` bernama `metode`; kolom waktu memakai `TIMESTAMP`.

## 4. Kebutuhan fungsional final (fase 1 selesai)

Donatur: register, login/logout, beranda, peta wilayah, lahan (grid petak), donasi, profile (+edit), donasi saya, penanaman saya.
Admin: lihat daftar donasi dan bukti bayar, terima/tolak, ubah status ke ditanam/selesai, unggah foto dokumentasi.

## 5. Status waterfall

| Fase | Status |
|---|---|
| 1. Analisis kebutuhan | Selesai |
| 2. Desain | **Sedang berjalan**: diagram kelas v2 selesai (selaras ERD revisi 1), ERD revisi 1 selesai (bisa direvisi lagi), peta navigasi halaman belum |
| 3. Implementasi | Belum |
| 4. Pengujian | Belum |
| 5. Deployment | Belum |
| 6. Pemeliharaan | Belum |

## 6. Diagram kelas (v2, lapisan Models)

```mermaid
classDiagram
  class Pengguna {
    <<abstract>>
    +int Id
    +string Nama
    +string Email
    -string passwordHash
    +VerifikasiPassword(password) bool
    +Peran()* string
  }
  class Donatur {
    +BuatDonasi(petak, pohon, jumlah, metode) Donasi
  }
  class Admin {
    +VerifikasiDonasi(donasi) void
    +UnggahDokumentasi(donasi, file) void
  }
  class Wilayah {
    +int Id
    +string Nama
    +JumlahTerisi() int
  }
  class Petak {
    +int Id
    +int Nomor
    +bool Terisi
  }
  class Pohon {
    +int Id
    +int Harga
  }
  class Donasi {
    +int Id
    +string NamaDonatur
    +string Pesan
    +int JumlahPohon
    +StatusDonasi Status
    +HitungTotal() int
    +Terima() void
    +Tolak() void
    +Tanam() void
    +Selesaikan() void
  }
  class Pembayaran {
    +int Id
    +int Jumlah
    +string BuktiBayarPath
    +DateTime DibayarPada
    +AmbilInstruksi() string
  }
  class MetodePembayaran {
    <<abstract>>
    +int Id
    +string Metode
    +AmbilInstruksi()* string
  }
  class TransferBank
  class EWallet
  class Qris
  class StatusDonasi {
    <<enumeration>>
    Menunggu
    Diterima
    Ditanam
    Selesai
    Ditolak
  }
  class Dokumentasi {
    +int Id
    +string PathFoto
    +DateTime Diunggah
  }
  Pengguna <|-- Donatur
  Pengguna <|-- Admin
  MetodePembayaran <|-- TransferBank
  MetodePembayaran <|-- EWallet
  MetodePembayaran <|-- Qris
  Wilayah "1" *-- "100" Petak : memiliki
  Donatur "1" --> "*" Donasi : membuat
  Donasi "*" --> "1" Petak : untuk
  Donasi "*" --> "1" Pohon : memakai harga
  Donasi "1" *-- "0..1" Pembayaran : dibayar lewat
  Pembayaran "*" --> "1" MetodePembayaran : memakai
  Donasi --> StatusDonasi
  Donasi "1" *-- "*" Dokumentasi
  Admin ..> Donasi : memverifikasi
```

Perubahan dari v1: ditambah `Pohon` dan `Pembayaran`; `MetodePembayaran` kini dipegang `Pembayaran` (bukan `Donasi`); `Wilayah.Kode` dihapus; `Donasi.Total` menjadi method `HitungTotal()` (= `JumlahPohon × Pohon.Harga`, nilainya disalin ke `Pembayaran.Jumlah` saat donasi dibuat).

Pemetaan 4 pilar OOP:
- **Encapsulation**: `passwordHash` private; `Donasi.Status` hanya berubah lewat `Terima/Tolak/Tanam/Selesaikan` dengan aturan urutan; `Petak.Terisi` tidak bisa diubah sembarangan.
- **Inheritance**: `Pengguna` → `Donatur`, `Admin`.
- **Polymorphism**: `MetodePembayaran` → `TransferBank`, `EWallet`, `Qris` (override `AmbilInstruksi()`). Di database hanya ada tabel `metode_pembayaran` (id, metode); kelas konkretnya dipilih dari nama metode saat data dibaca (misalnya lewat factory sederhana, belum diputuskan).
- **Abstraction**: kelas abstract `Pengguna`, `MetodePembayaran`, dan interface Repository.

Lapisan lain (usulan, belum digambar):
- Repositories: `IPenggunaRepository`, `IWilayahRepository` (termasuk petak), `IDonasiRepository` (termasuk pembayaran dan dokumentasi), plus satu repository untuk data referensi (`pohon`, `metode_pembayaran`); implementasi `Pg...Repository` (Npgsql).
- Services: `AuthService` (register/login/hash), `LahanService`, `DonasiService`.
- Pages WinUI hanya memanggil Services; tidak ada SQL di code-behind.

## 7. ERD (revisi 1)

```mermaid
erDiagram
  PENGGUNA ||--o{ DONASI : membuat
  WILAYAH ||--|{ PETAK : memiliki
  PETAK ||--o{ DONASI : "dipilih untuk"
  POHON ||--o{ DONASI : "dipilih dalam"
  DONASI ||--o| PEMBAYARAN : "dibayar lewat"
  METODE_PEMBAYARAN ||--o{ PEMBAYARAN : "dipakai pada"
  DONASI ||--o{ DOKUMENTASI : memiliki
  PENGGUNA ||--o{ DOKUMENTASI : "diunggah oleh"
  PENGGUNA {
    int id PK
    string nama
    string email UK
    string password_hash
    string peran
    timestamp dibuat_pada
  }
  WILAYAH {
    int id PK
    string nama
  }
  PETAK {
    int id PK
    int wilayah_id FK
    int nomor
  }
  POHON {
    int id PK
    int harga
  }
  METODE_PEMBAYARAN {
    int id PK
    string metode UK
  }
  DONASI {
    int id PK
    int pengguna_id FK
    int petak_id FK
    int pohon_id FK
    string nama_donatur
    string pesan
    int jumlah_pohon
    string status
    timestamp dibuat_pada
    timestamp diterima_pada
    timestamp ditanam_pada
    timestamp selesai_pada
  }
  PEMBAYARAN {
    int id PK
    int donasi_id FK
    int metode_pembayaran_id FK
    int jumlah
    string bukti_bayar_path
    timestamp dibayar_pada
  }
  DOKUMENTASI {
    int id PK
    int donasi_id FK
    int diunggah_oleh FK
    string path_foto
    timestamp diunggah_pada
  }
```

Catatan ERD:
- Aturan "satu petak satu donasi aktif" diterapkan lewat partial unique index `uq_donasi_petak_aktif` (`WHERE status <> 'ditolak'`) di `schema.sql`; tidak terlihat di diagram.
- Satu donasi punya paling banyak satu pembayaran (`pembayaran.donasi_id` UNIQUE).
- `pembayaran.jumlah` menyimpan nominal yang dibayar saat itu, sehingga riwayat tidak berubah bila `pohon.harga` diganti. Bisa dihapus bila ingin tanpa data ganda (total dihitung dari `jumlah_pohon × pohon.harga`).
- `peran` dan `status` berupa teks dengan `CHECK`.
- Status verifikasi tetap di `donasi.status`, bukan di `pembayaran`.
- `schema.sql` (8 tabel + data awal: 3 wilayah, 300 petak, harga pohon 15000, 3 metode bayar) **belum diuji di PostgreSQL**. Versi buatan pengguna sempat punya koma berlebih di tabel `pohon` (syntax error) dan `id` tanpa `IDENTITY`; versi di `schema.sql` sudah memperbaikinya (pengembalian `IDENTITY` menunggu konfirmasi pengguna).

## 8. Pertanyaan terbuka dan hal yang akan direvisi

1. Konfirmasi: `id` tabel memakai `IDENTITY` (otomatis) atau diisi manual?
2. Revisi ERD lanjutan oleh pengguna (nama/jumlah wilayah, `dokumentasi` per donasi atau per petak, perlu kolom `instruksi` di `metode_pembayaran` atau hardcode di kelas).
3. `Petak.Terisi` bergantung pada ada tidaknya donasi aktif: setuju? (belum dijawab)
4. Cara membuat peta Indonesia interaktif di WinUI.
5. Link GitHub belum ada.
6. Dosen/tugas memiliki format dokumen atau syarat OOP tertentu? (belum dijawab)

## 9. Langkah berikutnya

1. Peta navigasi halaman WinUI (termasuk Register dan Admin).
2. Jalankan `schema.sql` di database `terragenesis` dan cek tabel `petak` berisi 300 baris.
3. Tutup fase Desain → Implementasi: tambah Npgsql lewat NuGet, buat Models, Repository, Services, lalu Page pertama (Login).
