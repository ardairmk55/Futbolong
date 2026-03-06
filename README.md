# 🏆 Süper Lig FM 2026 - Match Engine Pro

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WinForms](https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Academic Project](https://img.shields.io/badge/Academic-Project-success?style=for-the-badge)

**Gümüşhane Üniversitesi - Bilgisayar Programcılığı** bölümü kapsamında geliştirilmiş, Nesne Yönelimli Programlama (OOP) prensiplerini derinlemesine kullanan, olay güdümlü (Event-Driven) bir futbol simülasyon ve maç motoru (Match Engine) projesidir.

---

## 📑 İçindekiler
- [Proje Hakkında](#-proje-hakkında)
- [Mimari ve OOP Yaklaşımı](#-mimari-ve-oop-yaklaşımı)
- [Oyun Motoru Özellikleri (Match Engine)](#-oyun-motoru-özellikleri-match-engine)
- [Kullanıcı Arayüzü (UI) ve Deneyim (UX)](#-kullanıcı-arayüzü-ui-ve-deneyim-ux)
- [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma)
- [Geliştirici Ekip](#-geliştirici-ekip)

---

## 🚀 Proje Hakkında

Süper Lig FM 2026, basit rastgele (random) sayılara dayalı klasik simülasyonların aksine; sahadaki her oyuncunun mevkisine, fiziksel durumuna ve maçın o anki momentumuna göre kararlar alan dinamik bir maç motorudur. Proje, güncel (2024-2025) Süper Lig kadrolarını 11'e 11 formatında simüle eder.

Bu proje, akademik gereksinimler doğrultusunda **Yapay Zeka destekli Pair-Programming** (Eşli Programlama) metodolojisi kullanılarak geliştirilmiş, prompt mühendisliği ile kod mimarisi inşa edilmiştir.

---

## 🧠 Mimari ve OOP Yaklaşımı

Proje, yazılım mühendisliği standartlarına uygun olarak katmanlı bir OOP mimarisiyle inşa edilmiştir:

* **Kalıtım (Inheritance):** Tüm futbolcular temel `Futbolcu` sınıfından türetilmiştir. Sahadaki her birey; isim, kondisyon, moral ve mevkiye özel yetenek puanlarını bu temel sınıftan miras alır.
* **Çok Biçimlilik (Polymorphism):** `PasAt()`, `CalimAt()` ve `SutAt()` gibi eylemler temel sınıfta `virtual` olarak tanımlanmış, `Forvet`, `Ortasaha` ve `Defans` alt sınıflarında yetenek setlerine göre ezilmiştir (override). `MatchEngine` oyuncunun tipini bilmeden temel sınıf üzerinden bu metotları polimorfik olarak çağırır.
* **Kapsülleme (Encapsulation):** Takım kadroları, oyuncu istatistikleri ve maçın anlık durumları (State) dışarıdan doğrudan erişime kapatılmış, sadece izin verilen metotlar (`HamleYap`, `OynayabilirMi`) üzerinden manipüle edilmiştir.
* **Bellek Yönetimi (Memory Management):** `Random` sınıfı gibi tekrar eden nesneler statik (`protected static readonly`) olarak tanımlanarak bellek sızıntıları (Memory Leak) önlenmiştir.

---

## ⚙️ Oyun Motoru Özellikleri (Match Engine)

1.  **Saha Zonlama (Zoning) Algoritması:** Saha 3 ana bölgeye ayrılmıştır (0: Defans, 1: Orta Saha, 2: Hücum). Top hücum bölgesine taşınmadan "Şut Çek" eylemine izin verilmez, mantıksal kısıtlamalar uygulanır.
2.  **Dinamik Zaman ve Uzatma Sistemi (Stoppage Time):** Maç dakikası yapılan eylemin türüne göre (Çalım: +2dk, Pas: +1dk) asimetrik akar. 45. ve 90. dakikalarda sistem rastgele uzatma dakikaları (`45+3`, `90+4`) belirler.
3.  **Durum Makinesi (State Machine):** Oyun, "İlk Yarı", "Devre Arası", "İkinci Yarı" ve "Maç Sonu" gibi farklı *state*'ler arasında geçiş yapar. Devre arasında tüm UI kontrolleri kilitlenir ve santra beklenir.
4.  **Momentum Çarpanı:** Bir takım üst üste başarılı pas/çalım yaptığında "Baskı/Momentum" değeri artar ve sonraki hamlelerin başarı şansını pozitif yönde etkiler.
5.  **Dinamik Spiker Modülü:** Spiker anlatımı statik değildir. Hazırlanan String Array havuzlarından rastgele çekilen cümlelerle her maç benzersiz bir anlatım sunar.

---

## 🎨 Kullanıcı Arayüzü (UI) ve Deneyim (UX)

* **Hard-Coded Runtime UI:** Visual Studio Designer üzerinde yaşanabilecek nesne çakışmalarını (CS1061, CS0102) önlemek adına form üzerindeki paneller, butonlar ve radarlar çalışma zamanında (Runtime) dinamik olarak oluşturulmuştur.
* **Samsunspor Splash Screen:** Proje açılışında marka kimliğini yansıtan özel bir animasyonlu yükleme ekranı mevcuttur.
* **FM Dark Theme:** Modern, göz yormayan karanlık tema (RGB 18, 18, 22) ve renk kodlu (Ev Sahibi: Crimson, Deplasman: DodgerBlue) paneller kullanılmıştır.
* **Saha Radarı:** Topun o an hangi bölgede olduğunu gösteren dinamik `ProgressBar` ve durum etiketleri mevcuttur.
* **Validation (Doğrulama):** Lobi ekranında iki oyuncunun aynı takımı seçmesi algoritmik olarak engellenmiş, hatalı seçimde butonlar kilitlenmiştir.

---

## 🛠️ Kurulum ve Çalıştırma

1. Projeyi bilgisayarınıza klonlayın veya `.zip` dosyasını klasöre çıkartın.
2. `Futbol.sln` dosyasını **Visual Studio 2019 veya üzeri** bir sürümle açın.
3. Çözüm Gezgini (Solution Explorer) üzerinden projeye sağ tıklayıp `Rebuild Solution` (Çözümü Yeniden Derle) seçeneğini tıklayın.
4. `Start` (Başlat) butonuna basarak veya `F5` tuşu ile projeyi çalıştırın.

*(Not: Proje herhangi bir harici kütüphane/Nuget paketi gerektirmez. Saf .NET Framework WinForms kullanılarak geliştirilmiştir.)*

---




**Tarih:** Mart 2026

> *"Futbol basit bir oyundur, zor olan onu basit oynamaktır." - Johan Cruyff*
