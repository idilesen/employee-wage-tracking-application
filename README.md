# Employee Wage Tracking Application (EWTA)

Employee Wage Tracking Application (EWTA), modern işletmelerin personel yönetimini, maaş/hakediş süreçlerini ve mali raporlamalarını tek bir merkezden yürütmesi için geliştirilmiş web tabanlı bir otomasyon sistemidir.

---

## 👥 Geliştirici Ekibi (Project Contributors)

Bu proje, aşağıda adı geçen geliştirici ekibi tarafından ortaklaşa tasarlanmış, kodlanmış ve hayata geçirilmiştir:

* **İdil Esen** (Software Engineer)
* **Talya Kuvvet** (Software Engineer)
* **Melis Can** (Software Engineer)

---

## 🚀 Projenin Amacı & Teknik Mimari

EWTA; dinamik veri girişi, esnek veri güncelleme mekanizmaları ve gelişmiş veri görselleştirme araçlarını bir araya getirerek insan kaynakları ve finans departmanlarının iş yükünü azaltmayı hedefler.

### Teknolojik Altyapı: ASP.NET Core Razor Pages
Proje, yüksek performans, platform bağımsız (cross-platform) çalışma yeteneği ve modern güvenlik standartları gereği **ASP.NET Core Razor Pages** mimarisi kullanılarak geliştirilmiştir. Klasik katı yapılar yerine, her sayfanın kendi veri modelini ve mantığını yönettiği modüler bir yapı tercih edilmiştir.

* **Arayüz Katmanı:** `.cshtml` (Razor Views) ile temiz ve scannable kullanıcı deneyimi.
* **Mantıksal Katman (Backend):** `.cshtml.cs` (PageModels) üzerinde asenkron C# kodları.
* **Yapılandırma:** `appsettings.json` ve `Program.cs` ile dinamik servis ve veritabanı yönetimi.

---

## 📁 Sistem Modülleri ve Fonksiyonel Yapı

Uygulama, veri bütünlüğünü ve kullanıcı kolaylığını ön planda tutan **19 farklı fonksiyonel sayfadan (38 ana kod dosyası)** oluşmaktadır:

### 1. Veri Yönetimi & CRUD Operasyonları (Forms)
* **Departman Yönetimi (`DepartmentEntry`):** Şirket içi departmanların tanımlanması, güncellenmesi ve listelenmesi.
* **Personel Kayıt & Takip (`EmployeeEntry` & `EmployeeList`):** Detaylı personel kartlarının oluşturulması ve departman bazlı filtrelenmesi.
* **Maaş & Hakediş Yönetimi (`WageEntry` & `WageUpdate`):** Personel maaş, komisyon ve ek ödemelerinin girilmesi. Dinamik veri tabloları (Grid yapısı) üzerinden anlık güncelleme ve silme (Edit/Delete) imkanı sunar.
* **Sihirbaz Formu (`WizardForm`):** Karmaşık veri giriş süreçlerini adımlara bölerek kullanıcı hatasını minimuma indiren akıllı veri giriş arayüzü.

### 2. Gelişmiş Raporlama & Veri Görselleştirme (Reports)
* **Yıllık Maaş Grafiği (`WageGraphYear`):** Şirketin yıllara göre toplam mali yükünü dinamik sütun grafiklerine döken görsel analiz katmanı.
* **Personel Maaş Grafiği (`WageGraphEmp`):** Çalışanların toplam hakedişlerini birbiriyle kıyaslayan ve bütçe planlamasını kolaylaştıran grafik modülü.
* **Departman Bazlı Raporlama (`DeptReport`):** Hangi departmanda kaç personelin aktif çalıştığını ve departman bütçelerini gösteren özet matris.
* **İletişim Rehberi (`Communication`):** Hızlı arama ve filtreleme destekli, personel iletişim bilgilerini listeleyen kurumsal rehber.
* **İşten Ayrılma Raporu (`ResignReport`):** Şirketten ayrılan personellerin istatistiksel takibi.

---

## 🛠️ Kurulum ve Yerel Çalıştırma

Proje .NET Core altyapısında olduğu için Windows, macOS veya Linux işletim sistemlerinde sorunsuz çalıştırılabilir:

1. Depoyu yerel bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/idilesen/employee-wage-tracking-application]