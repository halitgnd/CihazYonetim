# 🛡️ Cihaz Yönetim Paneli ve Canlı Ağ Simülasyonu

Bu proje, bir ağ üzerindeki cihazların (Router, Bilgisayar, Yazıcı vb.) durumlarını, konumlarını ve birbirleriyle olan bağlantılarını anlık olarak takip edip yönetebilmeyi sağlayan web tabanlı bir komuta merkezidir.

## 🚀 Projenin Amacı
Ağ yöneticilerinin veya operatörlerin;
- Sistemdeki cihazları tek bir ekranda görmesini,
- Cihazların "Online", "Offline", "Arızalı" veya "Bakım" durumlarını anlık takip etmesini,
- Cihaz arızalarını görsel uyarılarla (kırmızı yanıp sönen sinyaller) anında tespit etmesini sağlamaktır.

## 🛠️ Kullanılan Teknolojiler
- **Backend:** C#, ASP.NET Core MVC
- **Veritabanı:** SQLite & Entity Framework Core (Code-First)
- **Frontend Tasarım:** HTML5, Özel CSS (Karanlık Tema ve Cam Efektleri - Glassmorphism)
- **Frontend Mantık:** JavaScript, jQuery
- **Simülasyon / Animasyon:** jQuery UI (Sürükle-Bırak dinamikleri) ve dinamik SVG çizimleri.

## 🎯 Öne Çıkan Özellikler
- **Dinamik Ağ Çizimi:** Cihazlar ekranda sürüklendiğinde, en yakın Router (Yönlendirici) cihazına olan bağlantı kabloları anlık olarak yeniden çizilir.
- **Canlı Veri Akışı:** Cihazlar online olduğunda bağlantı kabloları üzerinden veri akışı animasyonu gösterilir.
- **Rol Bazlı Yetkilendirme:** Sistemde "Baş Yönetici" ve "Operatör" ayrımları bulunmaktadır. (Sadece yetkili kişiler yeni cihaz tanımlayabilir).
- **Zırhlı Arayüz:** Kullanıcı deneyimini artırmak için responsive (esnek) ve karanlık komuta merkezi teması tasarlanmıştır.

## 💻 Kurulum ve Çalıştırma
1. Projeyi bilgisayarınıza klonlayın.
2. Terminal (veya Package Manager Console) üzerinden veritabanını oluşturmak için `dotnet ef database update` komutunu çalıştırın.
3. Projeyi `dotnet run` komutu ile veya IDE'niz (Rider / Visual Studio) üzerinden başlatın.
