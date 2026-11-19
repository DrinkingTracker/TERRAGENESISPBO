# Graph Report - TERRAGENESIS  (2026-11-19)

## Corpus Check
- Corpus is ~38,209 words - fits in a single context window. You may not need a graph.

## Summary
- 300 nodes · 359 edges · 41 communities (19 shown, 22 thin omitted)
- Extraction: 83% EXTRACTED · 17% INFERRED · 1% AMBIGUOUS · INFERRED: 60 edges (avg confidence: 0.82)
- Token cost: 404,981 input · 0 output

## Community Hubs (Navigation)
- MainPage & WinUI Types
- App Namespaces & Code-Behind
- Domain Models & ERD
- Login & Project Checkpoints
- Donation Flow Mockups
- Beranda & User Page Mockups
- LahanCard Control
- BerandaPage XAML
- PostgreSQL Schema
- App Startup & MainWindow
- Navbar Control
- Admin Dashboard Mockups
- Project Build Config
- Payment Method Icons
- Beranda Region Photos
- Planting Stage Icons
- Leaf & Login Backgrounds
- Donasi Region Photos
- Password Field Icons
- Home Stat Icons
- Logo Duplicates (Beranda/Login)
- Profile & User Icons
- Tree Icon Duplicates
- Logo Duplicates (Donasi/Peta)
- Avatar Duplicates
- Store vs Brand Logo
- Donation Money Icons
- Profile Picture Icons
- Arrow Icon
- LockScreen Placeholder
- Login Arrow Icon
- SplashScreen Placeholder
- Square150 Placeholder
- Square44 Placeholder
- Square44 t24 Placeholder
- Square44 t48 Placeholder
- Alert Icon
- Hand Holding Icon
- Logout Icon
- Upload Icon
- Wide310 Placeholder

## God Nodes (most connected - your core abstractions)
1. `donasi` - 12 edges
2. `Page` - 11 edges
3. `Page` - 10 edges
4. `LahanCard` - 9 edges
5. `NavbarControl` - 9 edges
6. `BerandaPage` - 9 edges
7. `LoginPage` - 8 edges
8. `UserControl` - 6 edges
9. `Donasi (model class)` - 6 edges
10. `Donasi Page Mockup` - 6 edges

## Surprising Connections (you probably didn't know these)
- `Figma Admin Screens (Dashboard, Kelola Lahan, Kelola Donasi, Riwayat Penanaman)` --rationale_for--> `petak.status (aktif/nonaktif)`  [INFERRED]
  Assets/checkpoint 4.md → catatan.md
- `Laporan Feature Deferred` --rationale_for--> `Figma Admin Screens (Dashboard, Kelola Lahan, Kelola Donasi, Riwayat Penanaman)`  [INFERRED]
  catatan.md → Assets/checkpoint 4.md
- `Donasi Province Rollout Note` --conceptually_related_to--> `Donasi Page Design (frame 5:1090)`  [INFERRED]
  Assets/Donasi/note.md → catatan.md
- `Donasi Province Rollout Note` --references--> `Five Regions (Sumatra, Kalimantan, Sulawesi, Papua, Jawa Timur)`  [INFERRED]
  Assets/Donasi/note.md → catatan.md
- `Lahan Province Rollout Note` --references--> `Five Regions (Sumatra, Kalimantan, Sulawesi, Papua, Jawa Timur)`  [INFERRED]
  Assets/Lahan/note.md → catatan.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Layered Architecture: Pages -> Services -> Repositories -> PostgreSQL** — catatan_layered_architecture_rule, catatan_services_layer, catatan_repository_layer, catatan_postgresql_npgsql [EXTRACTED 1.00]
- **Per-province Asset Notes (Donasi, Lahan, Detail Lahan)** — assets_donasi_note_province_rollout, assets_lahan_note_province_rollout, assets_detaillahan_note_province_rollout, catatan_five_wilayah [INFERRED 0.85]
- **Beranda UI Composition** — assets_checkpoint_4_berandapage, assets_checkpoint_4_navbarcontrol, assets_checkpoint_4_lahancard, assets_checkpoint_4_controls_folder_rule [EXTRACTED 1.00]
- **Admin dashboard screens sharing sidebar layout** — assets_dashboard_admin_dashboard_admin_sample, assets_dashboard_admin_kelola_donasi_sample, assets_dashboard_admin_kelola_lahan_sample, assets_dashboard_admin_riwayat_penanaman_sample [EXTRACTED 1.00]
- **Searchable paginated admin table pattern** — assets_dashboard_admin_kelola_donasi_sample_donation_approval_table, assets_dashboard_admin_kelola_lahan_sample_land_plot_crud_table, assets_dashboard_admin_riwayat_penanaman_sample_planting_history_table [INFERRED 0.85]
- **Tree donation flow: pick region -> pick plot -> donate -> view planted tree** — assets_petawilayah_peta_wilayah_sample, assets_lahan_lahan_sample, assets_donasi_donasi_sample, assets_detaillahan_detaillahan_sample [INFERRED 0.85]
- **User Dashboard Pages sharing sidebar navigation** — assets_user_profile_sample, assets_user_donasi_saya_sample, assets_user_pesanan_saya_sample, assets_user_profile_sample_user_sidebar [EXTRACTED 1.00]
- **Donor Journey: login, browse land, donate, track planting** — assets_login_sample_login, assets_beranda_sampel_beranda, assets_user_donasi_saya_sample, assets_user_pesanan_saya_sample [INFERRED 0.75]
- **Donation Payment Method Icons** — assets_donasi_bankicon, assets_donasi_walleticon, assets_donasi_qricon [INFERRED 0.85]
- **Reforestation Region Photos (Jawa Timur, Kalimantan, Sumatera Utara)** — assets_donasi_jawatimur, assets_donasi_kalimantan, assets_donasi_sumaterautara [INFERRED 0.85]
- **Home page region photo set (Jawa Timur, Kalimantan, Sumatera Utara)** — assets_beranda_jawatimur, assets_beranda_kalimantan, assets_beranda_sumaterautara [INFERRED 0.85]
- **Login form field icons (username, password, visibility toggle, submit)** — assets_login_user, assets_login_padlock, assets_login_eye, assets_login_eye_off, assets_login_login_arrow [INFERRED 0.85]
- **Windows app-package tile/splash logos (unchanged default WinUI placeholders)** — assets_lockscreenlogo_scale_200, assets_splashscreen_scale_200, assets_square150x150logo_scale_200, assets_square44x44logo_scale_200, assets_square44x44logo_targetsize_24_altform_unplated, assets_square44x44logo_targetsize_48_altform_lightunplated, assets_storelogo, assets_wide310x150logo_scale_200 [INFERRED 0.95]
- **Planting growth-stage icons (seed, propagule, tree)** — assets_user_seedicon, assets_user_propagule, assets_user_treeicon [INFERRED 0.75]
- **White-outline user sidebar navigation icons** — assets_user_handicon1, assets_user_handicon2, assets_user_profileicon, assets_user_logout [INFERRED 0.75]

## Communities (41 total, 22 thin omitted)

### Community 0 - "MainPage & WinUI Types"
Cohesion: 0.07
Nodes (20): Page, MainPage, BerandaPage, EmailTextBox, EyeImage, IngatSayaCheckBox, LupaPasswordButton, MasukButton (+12 more)

### Community 1 - "App Namespaces & Code-Behind"
Cohesion: 0.11
Nodes (3): TERRAGENESIS.Controls, TERRAGENESIS.Pages, TERRAGENESIS

### Community 2 - "Domain Models & ERD"
Cohesion: 0.09
Nodes (22): Checkpoint 2 (post ERD revision 1), Class Diagram v2 (Models layer), Dokumentasi (planting photo model), Donasi (model class), ERD Revision 1, Pembayaran (payment model), Petak (plot model), Pohon (tree price catalog) (+14 more)

### Community 3 - "Login & Project Checkpoints"
Cohesion: 0.10
Nodes (17): StatusDonasi Enumeration, Checkpoint 3 (after LoginPage + GitHub repo), Temporary Font Resources (FontBlack/FontBold/FontIsi), Login Placeholder Assets (Assets/login), LoginPage (UI only), MainWindow RootFrame, Page Navigation Map, BerandaPage (+9 more)

### Community 4 - "Donation Flow Mockups"
Cohesion: 0.15
Nodes (19): Detail Lahan Mockup (planted plot popup), Pohon Harapan Detail Card (tanggal donasi, nama donatur, lokasi, pesan), Donasi Page Mockup, Donor Form (Nama Donatur, Pesan/Harapan), Metode Pembayaran (Transfer Bank, E-Wallet, QRIS), Upload Bukti Transfer & Konfirmasi Donasi, Top Navigation Bar (Beranda, Lahan, Donasi, Profile), Jumlah Pohon Stepper & Total Donasi (Rp 15.000 per tree) (+11 more)

### Community 5 - "Beranda & User Page Mockups"
Cohesion: 0.13
Nodes (18): Aerial Forest Hero Background Image, Beranda (Home) Page Mockup, Hero Section 'Satu Pilihan, Tumbuhkan Kehidupan' with Jelajahi Lahan CTA, Impact Stats Card (Total Pohon Tertanam, Donatur Peduli, Wilayah Terhijaukan), Lahan Unggulan Featured Land Cards (region + available slots), Top Navbar (Beranda/Lahan/Donasi/Profile + avatar), Login Page Mockup, Login Brand Panel ('Hijaukan Bumi, Mulai dari Sekarang') (+10 more)

### Community 6 - "LahanCard Control"
Cohesion: 0.12
Nodes (12): BrushGambar, Root, TxtJudul, TxtSlot, UserControl, LahanCard, Gambar, Judul (+4 more)

### Community 7 - "BerandaPage XAML"
Cohesion: 0.16
Nodes (15): BtnJelajah, CardJatim, CardKalimantan, CardSumut, Kanvas, KartuStatistik, Page, TxtDonatur (+7 more)

### Community 8 - "PostgreSQL Schema"
Cohesion: 0.31
Nodes (15): dokumentasi, donasi, ix_dokumentasi_donasi, ix_donasi_pengguna, ix_donasi_status, metode_pembayaran, pembayaran, pengguna (+7 more)

### Community 9 - "App Startup & MainWindow"
Cohesion: 0.20
Nodes (6): Application, App, RootFrame, Window, MainWindow, Frame

### Community 10 - "Navbar Control"
Cohesion: 0.20
Nodes (5): GarisAktif, UserControl, NavbarControl, ActivePage, Rectangle

### Community 11 - "Admin Dashboard Mockups"
Cohesion: 0.27
Nodes (11): Dashboard Admin Mockup, Admin Sidebar Navigation (Dashboard, Kelola Lahan, Kelola Donasi, Riwayat Penanaman, Laporan, Keluar), Aktivitas Baru (recent activity feed), Peta Wilayah (Indonesia region map), Ringkasan Laporan Sistem (KPI stat cards), Kelola Donasi Mockup, Donation Approval Table, Kelola Lahan Mockup (+3 more)

### Community 12 - "Project Build Config"
Cohesion: 0.29
Nodes (6): net10.0-windows10.0.26100.0, Microsoft.Windows.SDK.BuildTools (*), Microsoft.Windows.SDK.BuildTools.WinApp (*), Microsoft.WindowsAppSDK (*), Npgsql (10.0.3), Microsoft.NET.Sdk

### Community 13 - "Payment Method Icons"
Cohesion: 0.40
Nodes (5): Alert/Success Check Icon (Donasi), Bank Building Icon (bank transfer payment method), QR Code Icon (QRIS payment method), Cloud Upload Icon (payment proof upload), Wallet Icon (e-wallet payment method)

### Community 14 - "Beranda Region Photos"
Cohesion: 0.50
Nodes (4): Jawa Timur Region Photo (tropical forest banner), Kalimantan Region Photo (misty rainforest banner), Pin Icon (black map location marker for region cards), Sumatera Utara Region Photo (jungle banner)

### Community 15 - "Planting Stage Icons"
Cohesion: 0.50
Nodes (4): Leaf Badge Icon (green leaves in circle), Propagule Icon (green sprout in circle badge), Seed/Seedling Icon (sprout in soil), Tree Icon (mature green tree)

### Community 16 - "Leaf & Login Backgrounds"
Cohesion: 0.67
Nodes (3): Daun Icon (green two-leaf sprout, home page), Leaf Overlay (hanging leafy branch, decorative corner), Login Background (sunny park with trees and lawn)

### Community 17 - "Donasi Region Photos"
Cohesion: 0.67
Nodes (3): Jawa Timur Forest Region Photo, Kalimantan Rainforest Region Photo, Sumatera Utara Misty Rainforest Region Photo

### Community 18 - "Password Field Icons"
Cohesion: 0.67
Nodes (3): Eye Icon (password visible state), Eye-Off Icon (password hidden state), Padlock Icon (password field)

## Ambiguous Edges - Review These
- `Alert/Success Check Icon (Donasi)` → `Cloud Upload Icon (payment proof upload)`  [AMBIGUOUS]
  Assets/Donasi/aleritcon.png · relation: conceptually_related_to
- `TERRAGENESIS Brand Logo (green globe with leaf sprout)` → `StoreLogo (default WinUI placeholder)`  [AMBIGUOUS]
  Assets/user/logo.png · relation: semantically_similar_to

## Knowledge Gaps
- **85 isolated node(s):** `Border`, `ImageBrush`, `Judul`, `JumlahSlot`, `Gambar` (+80 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 122 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **22 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `BerandaPage` connect `MainPage & WinUI Types` to `App Namespaces & Code-Behind`, `BerandaPage XAML`?**
  _High betweenness centrality (0.050) - this node is a cross-community bridge._
- **What connects `Border`, `ImageBrush`, `Judul` to the rest of the system?**
  _85 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `MainPage & WinUI Types` be split into smaller, more focused modules?**
  _Cohesion score 0.06612685560053981 - nodes in this community are weakly interconnected._
- **Why does `LoginPage` connect `MainPage & WinUI Types` to `App Namespaces & Code-Behind`?**
  _High betweenness centrality (0.050) - this node is a cross-community bridge._
- **Should `App Namespaces & Code-Behind` be split into smaller, more focused modules?**
  _Cohesion score 0.11375661375661375 - nodes in this community are weakly interconnected._

### Low-confidence Hints
_AMBIGUOUS edges — the extractor was unsure. Verify before acting on these._

- **What is the exact relationship between `Alert/Success Check Icon (Donasi)` and `Cloud Upload Icon (payment proof upload)`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._
- **What is the exact relationship between `TERRAGENESIS Brand Logo (green globe with leaf sprout)` and `StoreLogo (default WinUI placeholder)`?**
  _Edge tagged AMBIGUOUS (relation: semantically_similar_to) - confidence is low._