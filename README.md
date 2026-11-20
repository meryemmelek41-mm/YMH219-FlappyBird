# 🐦 Flappy Bird - YMH219

**Nesne Tabanlı Programlama (YMH 219)** dersi kapsamında geliştirilen Windows Forms tabanlı Flappy Bird oyunu.

## 📋 Proje Hakkında

Bu proje, Windows Forms üzerinde zamanlayıcı (Timer) ve klavye olaylarının kullanıldığı basit bir 2D oyundur. Görsel bileşenler çalışma zamanında hareket ettirilmekte, çarpışma kontrolü ve oyun durumu nesne tabanlı bir yapı ile yönetilmektedir.

## 🛠️ Kullanılan Teknolojiler

- **C# / .NET 6.0** - Programlama dili ve çatı
- **Windows Forms** - Masaüstü arayüz kütüphanesi
- **Visual Studio 2022** - Geliştirme ortamı
- **GDI+** - 2D grafik çizimi (System.Drawing)

## 🚀 Kurulum ve Çalıştırma

1. Bu depoyu klonlayın:
   ```bash
   git clone https://github.com/meryemmelek41-mm/YMH219-FlappyBird.git
   ```
2. `FlappyBird.sln` dosyasını Visual Studio ile açın.
3. `F5` tuşuna basarak veya **Debug > Start Debugging** ile çalıştırın.

**Gereksinimler:**
- Visual Studio 2022 veya üzeri
- .NET 6.0 SDK

## 🎮 Temel Özellikler

- **Kuş Kontrolü:** SPACE veya Yukarı Ok tuşu ile kuş yukarı zıplar; yer çekimi etkisiyle aşağı iner.
- **Engeller:** Borular ekranın sağından sola doğru hareket eder; ekran dışına çıkan borular otomatik olarak yeniden oluşturulur.
- **Çarpışma Kontrolü:** Kuş borulara, zemine veya tavana çarptığında oyun durur ve "Game Over" ekranı gösterilir.
- **Skor Sistemi:** Her başarılı boru geçişinde skor artar.
- **Zorluk Artışı:** Her 5 skorda boru hızı biraz artar.
- **En Yüksek Skor:** En iyi skor dosyaya kaydedilir ve kalıcı olarak saklanır.
- **Yeniden Başlatma:** Game Over sonrası tıklama veya SPACE ile oyun yeniden başlatılabilir.

## 🏗️ Proje Yapısı

```
FlappyBird/
├── FlappyBird.sln            # Solution dosyası
├── .gitignore                # Git ignore kuralları
├── README.md                 # Bu dosya
└── FlappyBird/
    ├── FlappyBird.csproj     # Proje dosyası
    ├── Program.cs            # Ana giriş noktası
    ├── FormMain.cs           # Ana form - oyun döngüsü ve çizim
    ├── FormMain.Designer.cs  # Form tasarım dosyası
    ├── Bird.cs               # Kuş sınıfı (konum, fizik, çizim)
    ├── Pipe.cs               # Boru sınıfı (engel yönetimi)
    └── GameState.cs          # Oyun durumu sınıfı (skor, durum)
```

## 📸 Ekran Görüntüleri

### Oyun Menüsü
*(Oyun başlatıldığında görülen ana menü ekranı)*

### Oynanış
*(Boruların arasından geçerken oyun ekranı)*

## ⚙️ Nesne Tabanlı Tasarım

| Sınıf | Sorumluluk |
|-------|-----------|
| `Bird` | Kuşun konumu, hızı, yer çekimi, zıplama ve çizim işlemleri |
| `Pipe` | Boru engelleri: konum, hareket, çarpışma alanı ve çizim |
| `GameState` | Oyun durumu (Menü/Oyun/GameOver), skor yönetimi, en iyi skor kaydı |
| `FormMain` | Oyun döngüsü (Timer), klavye/fare olayları, çarpışma kontrolü, ekran çizimi |

## ⚠️ Bilinen Sınırlamalar

- Oyun yalnızca Windows işletim sisteminde çalışır (Windows Forms bağımlılığı).
- Ses efektleri bulunmamaktadır.
- Grafikler kod ile çizilmektedir (harici resim dosyası kullanılmamıştır).

## 📝 Geliştirme Notları

- `DoubleBuffered = true` ile ekran titremesi (flicker) engellenmiştir.
- Çarpışma kontrolünde kuşun hitbox'ı biraz küçültülerek daha adil bir oynanış sağlanmıştır.
- GDI+ ile SmoothingMode.AntiAlias kullanılarak daha düzgün çizimler elde edilmiştir.

---

**Öğrenci:** Meryem Melek Şimşek  
**Öğrenci No:** 250541017  
**Ders:** Nesne Tabanlı Programlama (YMH 219)
