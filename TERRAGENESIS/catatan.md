# Catatan Proyek — TERRAGENESIS (PBO, WinUI 3)

Dibuat: 19 November 2026 (menggantikan `checkpoint 4.md`, untuk dibawa ke sesi Claude Code).
Taruh file ini (dan `schema.sql`) di root repo supaya terbaca otomatis oleh Claude Code.

## 1. Tujuan dan lingkungan

- Aplikasi OOP (PBO) dengan antarmuka **WinUI 3**, bahasa C#.
- Desain Figma: https://www.figma.com/design/HGco7zF20HmR3AXVUVJ1mf/PROJECT-PBO (file key `HGco7zF20HmR3AXVUVJ1mf`)
- Repo GitHub: https://github.com/DrinkingTracker/TERRAGENESISPBO. Status push/commit terakhir belum dikonfirmasi ulang di sesi ini.
- Visual Studio 2026 Community, workload **.NET desktop development** + **WinUI application development**.
- Proyek: **TERRAGENESIS (Package)**, konfigurasi Debug x64, disimpan di `D:\TERRAGENESIS\TERRAGENESIS`.
- Developer Mode Windows aktif. Proyek harus di drive **NTFS** (pernah gagal deploy `DEP0700` karena FAT32).
- Database: **PostgreSQL + pgAdmin**.
- Metodologi: **SDLC waterfall**.
- Catatan kerja: shell Claude (versi sebelumnya) tidak bisa mengunduh dari figma.com langsung; aset gambar diekspor manual oleh pengguna dari Figma.

## 2. Konsep aplikasi

Platform donasi penanaman pohon. Alur donatur: pilih wilayah di peta → pilih petak kosong di grid 10×10 → isi form donasi (jumlah pohon, nama, pesan, metode bayar, upload bukti) → admin verifikasi → penanaman → dokumentasi. Harga **Rp 15.000 per pohon**.

### Daftar layar (hasil fetch ulang Figma, sesi ini)

**Donatur:**
- Beranda (`1:7`) — hero, statistik, 3 kartu wilayah unggulan
- Login (`21:5`) — belum ada child node terbaca di metadata terbaru
- Peta wilayah (`4:80`) — peta Indonesia vektor per provinsi, 5 label wilayah
- Lahan (`5:187` dan `5:817`) — grid 10×10 + panel ringkasan + pop-up detail petak terisi
- Donasi (`5:1090`) — form donasi lengkap (lihat detail di bawah)
- Profile (`36:118`) — data diri + 3 kartu ringkasan + tombol Edit Profile
- Donasi Saya (`44:263`) — tabel riwayat donasi (Tanggal/Wilayah/Nominal/Status)
- Penanaman Saya / "Pesanan saya" (`44:550`) — timeline 3 tahap + dokumentasi foto

**Admin:**
- Dashboard Admin (`77:5`) — 4 kartu ringkasan + peta mini + Aktivitas Baru
- Kelola Lahan (`98:281`) — tabel petak (Lahan/Wilayah/Status/Aksi), CRUD
- Kelola Donasi (`110:732`) — tabel donasi, aksi setujui/tolak per baris, badge "Disetujui"
- Riwayat Penanaman (`115:1202`) — tabel Tanggal/Wilayah/Lahan/Jumlah pohon
- Laporan — menu ada di sidebar, **sengaja belum dikerjakan** (mungkin dibatalkan, lihat §3)

Register belum ada di Figma, dibuat mengikuti gaya Login.

### Detail penting per layar (temuan sesi ini)

**Halaman Donasi (`5:1090`):**
- Foto lahan + label "Tanam pohon baru" / "Petak #23 · Jawa Timur"
- Counter Jumlah Pohon (−/+), Total Donasi otomatis (jumlah × Rp 15.000)
- Field Nama Donatur, Pesan/Harapan Donatur
- Pilihan Metode Pembayaran: Transfer Bank / E-Wallet / QRIS (masing-masing expandable)
- Instruksi Pembayaran + area upload bukti ("Upload bukti transfer, **JPG/PNG, maksimal 5MB**" — validasi di Services, bukan constraint DB)
- Tombol submit: "Konfirmasi Donasi"

**Pop-up detail petak (di halaman Lahan, petak terisi):**
- Judul "Pohon Harapan", status "Petak #23 · Sudah ditanam"
- Isi: Tanggal Donasi, Nama Donatur, Lokasi Penanaman, kutipan Pesan dari Donatur
- Panel Ringkasan: 37/100 petak terisi (angka contoh Figma, bisa beda dari Dashboard)

**Kelola Donasi:**
- Kolom: Nama Donatur / Jumlah / Total Biaya / Bukti Bayar (thumbnail gambar) / Aksi
- Aksi: ikon centang/silang (belum diproses) atau badge hijau "**Disetujui**" (sudah diproses)
- **Istilah status final: "Disetujui"** (bukan "Diterima" yang sempat muncul di Donasi Saya — sudah dikonfirmasi pengguna)

**Kelola Lahan:**
- Kolom: Lahan / Wilayah / Status (Aktif/nonaktif) / Aksi (lihat, ubah, hapus)
- Paginasi "1-10 dari 500 data" → total petak = 500 (5 wilayah × 100)

**Riwayat Penanaman:**
- Kolom: Tanggal / Wilayah / Lahan / Jumlah pohon
- Beberapa petak bisa tampil di tanggal yang sama (dikelompokkan di tampilan saja, aturan 1 donasi = 1 petak tetap berlaku di data)

**Dashboard Admin:**
- Kartu: Total Pohon Ditanam, Lahan Tersedia, Total Donasi, Total Pengguna
- Definisi pasti tiap angka masih **asumsi**, belum dikonfirmasi satu per satu

**Profile:**
- Foto profil, nama, email, tombol Edit Profile
- 3 kartu: Total Pohon Dibeli / Total Donasi / Wilayah dipilih

**Nama 5 wilayah (dari label peta):** Sumatra, Kalimantan, Sulawesi, Papua, Jawa Timur.
*(Catatan: kartu Beranda sempat menyebut "Sumatera Utara", bukan "Sumatra" — belum direkonsiliasi sepenuhnya, sejauh ini schema.sql mengikuti label peta.)*

## 3. Keputusan yang sudah ditetapkan

1. Login sungguhan dengan database; ada halaman Register.
2. Layar admin lengkap: Dashboard, Kelola Lahan, Kelola Donasi, Riwayat Penanaman (Laporan ditunda/kemungkinan dibatalkan).
3. Verifikasi pembayaran **manual** (upload bukti transfer, validasi JPG/PNG maks 5MB di Services).
4. **Satu donasi = satu petak, boleh banyak pohon**, tanpa batas jumlah pohon per petak.
5. Satu petak hanya boleh dipegang satu donasi aktif; donasi `ditolak` membebaskan petak.
6. Alur status donasi: `menunggu` → `disetujui` → `ditanam` → `selesai`, atau `menunggu` → `ditolak`. **Istilah "disetujui" sudah final.**
7. Password disimpan sebagai hash; query database memakai parameter (bukan string concat).
8. Akun admin dibuat manual di database (tidak lewat register).
9. Penyimpanan: PostgreSQL via Npgsql.
10. `pohon`, `pembayaran`, `metode_pembayaran` adalah tabel terpisah; `pohon` hanya katalog harga (id, harga), bukan satu baris per pohon yang ditanam.
11. Kolom waktu pakai `TIMESTAMP`. Semua `id` pakai `IDENTITY`.
12. Petak punya kolom `status` (aktif/nonaktif) yang diatur admin — terpisah dari status terisi/kosong (yang dihitung dari donasi aktif, tidak disimpan sebagai kolom).
13. Struktur kode: folder `Models`, `Repositories`, `Services`, `Pages`, `Controls`. Halaman memakai `Page` di dalam satu `Frame` pada `MainWindow`. Page hanya memanggil Services, tidak ada SQL di code-behind.
14. Gambar disimpan di `Assets/` per halaman (`Assets/login/`, `Assets/beranda/`, dst).
15. `Controls` = komponen UI dipakai ulang (UserControl), tanpa pengetahuan soal halaman lain/database, komunikasi lewat properti & event. `Pages` = layar utuh.
16. **Fitur Laporan ditunda** — jangan dikerjakan dulu, kemungkinan dibatalkan.

## 4. Status waterfall

| Fase | Status |
|---|---|
| 1. Analisis kebutuhan | Selesai |
| 2. Desain | Diagram kelas v2 dan ERD revisi 1 ada, tapi **belum diperbarui** mengikuti temuan Figma sesi ini (layar admin lengkap, halaman Donasi, pop-up Lahan) |
| 3. Implementasi | Bagian donatur selesai (20 November 2026): Models, Repository, Services, dan semua halaman donatur. Belum dites ke database sungguhan. Area admin belum (logika admin sudah ada di Services) |
| 4. Pengujian | Belum |
| 5. Deployment | Belum |
| 6. Pemeliharaan | Belum |

## 5. Status database (schema.sql)

`schema.sql` revisi 3 sudah dibuat ulang mengikuti Figma terbaru, tapi **belum pernah dijalankan/diuji di PostgreSQL**. Entity final: `pengguna`, `wilayah`, `petak`, `pohon`, `metode_pembayaran`, `donasi`, `pembayaran`, `dokumentasi` — plus 3 VIEW opsional (`v_status_petak`, `v_dashboard`, `v_riwayat_penanaman`) sebagai alat bantu query, bukan bagian dari ERD/diagram kelas.

Yang masih perlu dicek/diputuskan:
- Jalankan skrip di database `terragenesis`, cek wilayah = 5, petak = 500.
- Definisi pasti angka-angka di `v_dashboard` (Total Donasi, Lahan Tersedia) masih asumsi.
- Nama wilayah final: "Sumatra" (label peta) vs "Sumatera Utara" (kartu Beranda) — belum direkonsiliasi.
- Apakah `IDENTITY` dipakai (sudah diputuskan ya di skrip, tapi belum eksplisit dikonfirmasi ulang pengguna).

## 6. Diagram kelas & ERD

**BELUM DIPERBARUI** mengikuti temuan Figma sesi ini (ada di `checkpoint 4.md` versi v2/revisi 1, masih relevan secara struktur tapi belum menyertakan status "Disetujui" final, kolom `petak.status`, dsb — meski `schema.sql` sudah mengikuti itu semua). Perlu diselaraskan ulang sebagai langkah berikutnya.

Pemetaan 4 pilar OOP yang disepakati:
- **Encapsulation**: `passwordHash` private; `Donasi.Status` hanya berubah lewat method `Terima/Tolak/Tanam/Selesaikan` dengan aturan urutan; `Petak.Terisi` tidak bisa diubah sembarangan (dihitung, bukan field).
- **Inheritance**: `Pengguna` → `Donatur`, `Admin`.
- **Polymorphism**: `MetodePembayaran` (abstract) → `TransferBank`, `EWallet`, `Qris`, override `AmbilInstruksi()`. Di database hanya ada tabel `metode_pembayaran` (id, metode); kelas konkret dipilih dari nama metode saat data dibaca (factory sederhana, detail belum diputuskan).
- **Abstraction**: kelas abstract `Pengguna`, `MetodePembayaran`, interface Repository.

Lapisan yang direncanakan (belum digambar ulang):
- Repositories: `IPenggunaRepository`, `IWilayahRepository` (+petak), `IDonasiRepository` (+pembayaran, dokumentasi), repository data referensi (pohon, metode_pembayaran). Implementasi `Pg...Repository` via Npgsql.
- Services: `AuthService` (register/login/hash), `LahanService`, `DonasiService`.

## 7. Progres implementasi (ringkasan)

- `MainWindow`: `Frame` bernama `RootFrame`, membuka `LoginPage` saat start.
- `Pages/LoginPage`: tampilan selesai, tanpa database. Validasi kolom kosong, ikon mata password, checkbox "Ingat Saya". `AuthService` belum ada.
- `Controls/NavbarControl`, `Controls/LahanCard`: dibuat untuk Beranda.
- `Pages/BerandaPage`: tampilan selesai, data statistik masih hardcode. Navigasi antar halaman masih dialog "Halaman belum dibuat".
- Font Figma (Metropolis, Gotham, Plus Jakarta Sans) belum dipasang, masih pakai Segoe UI sementara.
- Aset placeholder buatan Claude di `Assets/login/` perlu diganti aset asli dari Figma.

### Pembaruan 20 November 2026 (bagian donatur)

- **Models**: `Pengguna` (abstract) → `Donatur`, `Admin`; `Donasi` (status hanya lewat `Setujui/Tolak/Tanam/Selesaikan`); `MetodePembayaran` (abstract) → `TransferBank`, `EWallet`, `Qris` dengan `AmbilInstruksi()`; `Petak`, `Wilayah`, `Pohon`, `Pembayaran`, `Dokumentasi`; `PasswordHasher` (PBKDF2-SHA256).
- **Repositories**: interface `IPenggunaRepository`, `IWilayahRepository`, `IDonasiRepository`, `IReferensiRepository`; implementasi `Pg...Repository` (Npgsql, query berparameter). Koneksi lewat `Repositories/DatabaseConnection.cs`, dibuat di `Services/Layanan.cs` (database `TERRAGENESISPBO`; `schema.sql` revisi 3 + `seed_demo.sql` sudah dijalankan 20 November 2026).
- **Services**: `AuthService`, `LahanService`, `DonasiService`, `PenyimpananFoto` (validasi JPG/PNG maks 5MB, disalin ke folder data aplikasi), `Sesi`, `Layanan` (perakit service). Logika admin (verifikasi, ditanam, selesai, unggah dokumentasi, aktif/nonaktif petak) sudah ada, halamannya belum.
- **Pages**: Login (database sungguhan, "Ingat Saya" menyimpan email), Beranda (statistik dan slot dari database), PetaWilayah, Lahan (grid + pop-up), Donasi, Profile, DonasiSaya, PenanamanSaya. **Controls**: `SidebarControl` baru.
- Tidak dibuat karena belum ada desain Figma: Register, Lupa Password, form Edit Profile (tombolnya hanya menampilkan "belum tersedia").
- Akun demo: jalankan `seed_demo.sql` setelah `schema.sql` (donatur@terragenesis.id / donatur123).
- Kartu Beranda "Sumatera Utara" dipetakan ke wilayah database "Sumatra".

## 8. Pertanyaan terbuka

1. Isi frame Login (`21:5`) di Figma belum terbaca ulang di sesi ini (metadata kosong) — cek lagi kalau desainnya berubah.
2. Definisi pasti 4 angka Dashboard Admin.
3. Rekonsiliasi nama wilayah: "Sumatra" vs "Sumatera Utara".
4. Cara membuat peta Indonesia interaktif di WinUI (vektor per provinsi dari Figma, bagian tersulit).
5. Apakah commit terbaru sudah di-push ke GitHub dan `.gitignore` sudah benar (`bin`/`obj` tidak ikut)?
6. Apakah font Figma (.ttf) sudah tersedia untuk dipasang?
7. Dosen/tugas punya format dokumen atau syarat OOP tertentu? (belum dijawab)

## 9. Langkah berikutnya (urutan disarankan)

1. Jalankan `schema.sql` di pgAdmin pada database `terragenesis`, verifikasi 5 wilayah dan 500 petak.
2. Selaraskan ulang diagram kelas dan ERD dengan temuan Figma sesi ini (status "Disetujui", `petak.status`, halaman Donasi, dsb).
3. Tambah Npgsql lewat NuGet, buat Models, Repository, Services dasar.
4. Hubungkan LoginPage ke database sungguhan (`AuthService`).
5. Lanjutkan halaman yang belum dibuat: RegisterPage, Peta wilayah, Lahan (grid + pop-up), Donasi, Profile, Donasi Saya, Penanaman Saya — lalu area admin.
6. Ganti aset placeholder dan pasang font asli dari Figma.
