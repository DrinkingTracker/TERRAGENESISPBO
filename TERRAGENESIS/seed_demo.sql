-- ============================================================
-- TERRAGENESIS - seed_demo.sql
-- Akun donatur untuk mencoba aplikasi, karena halaman Daftar
-- belum ada di Figma (jadi belum dibuat). Jalankan SETELAH
-- schema.sql, di database `TERRAGENESISPBO` lewat pgAdmin.
--
--   Email    : donatur@terragenesis.id
--   Password : donatur123
--
-- password_hash dibuat dengan format yang sama seperti
-- Models/PasswordHasher.cs (PBKDF2-SHA256, 100.000 iterasi).
-- ============================================================

INSERT INTO pengguna (nama, email, password_hash, peran)
VALUES ('Natasya Zulaikha', 'donatur@terragenesis.id',
        'pbkdf2$100000$BOlmkIccJWpuyMCWJz22Gw==$13xf9wRlsMYxgjTOEQjvITMuyihubilffPDCmwrN2sY=',
        'donatur')
ON CONFLICT (email) DO NOTHING;
