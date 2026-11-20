using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace FlappyBird
{
    /// <summary>
    /// Kuş nesnesini temsil eden sınıf.
    /// Konum, hız, yer çekimi ve çizim işlemlerini yönetir.
    /// Harici bir kuş görseli (bird.png) kullanır.
    /// </summary>
    public class Bird
    {
        // --- Özellikler ---
        public float X { get; set; }
        public float Y { get; set; }
        public float Velocity { get; set; }
        public int Width { get; } = 45;
        public int Height { get; } = 35;
        public bool IsAlive { get; set; } = true;

        // --- Sabitler ---
        private const float Gravity = 0.5f;
        private const float JumpForce = -8f;
        private const float MaxFallSpeed = 10f;

        // Kuş görseli
        private Image birdImage;

        /// <summary>
        /// Kuş nesnesini başlangıç konumunda oluşturur ve görseli yükler.
        /// </summary>
        public Bird(float startX, float startY)
        {
            X = startX;
            Y = startY;
            Velocity = 0;
            IsAlive = true;
            LoadBirdImage();
        }

        /// <summary>
        /// Gömülü kaynak (embedded resource) olarak kuş görselini yükler.
        /// </summary>
        private void LoadBirdImage()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlappyBird.Resources.bird.png"))
                {
                    if (stream != null)
                    {
                        birdImage = Image.FromStream(stream);
                    }
                }
            }
            catch (Exception)
            {
                // Görsel yüklenemezse kod ile çizim yapılır
                birdImage = null;
            }
        }

        /// <summary>
        /// Kuşun çarpışma dikdörtgenini döndürür.
        /// </summary>
        public RectangleF GetBounds()
        {
            // Biraz küçültülmüş hitbox (daha adil oynanış)
            return new RectangleF(X + 4, Y + 4, Width - 8, Height - 8);
        }

        /// <summary>
        /// Kuşu zıplatır (yukarı hareket).
        /// </summary>
        public void Jump()
        {
            if (IsAlive)
            {
                Velocity = JumpForce;
            }
        }

        /// <summary>
        /// Her frame'de kuşun konumunu günceller.
        /// Yer çekimi uygulanır.
        /// </summary>
        public void Update()
        {
            if (!IsAlive) return;

            Velocity += Gravity;
            if (Velocity > MaxFallSpeed)
                Velocity = MaxFallSpeed;

            Y += Velocity;
        }

        /// <summary>
        /// Kuşu ekrana çizer. Görsel varsa resimle, yoksa kod ile çizer.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Dönüş açısı hesapla (velocity'ye göre)
            float angle = Velocity * 3f;
            angle = Math.Max(-25, Math.Min(angle, 70));

            // Dönüş merkezi
            float cx = X + Width / 2f;
            float cy = Y + Height / 2f;

            g.TranslateTransform(cx, cy);
            g.RotateTransform(angle);
            g.TranslateTransform(-cx, -cy);

            if (birdImage != null)
            {
                // Resimle çiz
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(birdImage, X, Y, Width, Height);
            }
            else
            {
                // Yedek: kod ile çizim (görsel bulunamazsa)
                DrawFallback(g);
            }

            g.ResetTransform();
        }

        /// <summary>
        /// Görsel yüklenemediğinde yedek olarak kod ile kuş çizer.
        /// </summary>
        private void DrawFallback(Graphics g)
        {
            // Gövde (pembe - yüklenen görsele benzer)
            using (var bodyBrush = new SolidBrush(Color.FromArgb(230, 150, 170)))
            {
                g.FillEllipse(bodyBrush, X, Y, Width, Height);
            }

            // Dış çerçeve (siyah)
            using (var outlinePen = new Pen(Color.Black, 2.5f))
            {
                g.DrawEllipse(outlinePen, X, Y, Width, Height);
            }

            // Göz (beyaz daire - büyük)
            using (var eyeWhiteBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(eyeWhiteBrush, X + Width - 22, Y + 2, 14, 16);
                g.FillEllipse(eyeWhiteBrush, X + Width - 14, Y + 4, 14, 16);
            }

            // Göz çerçevesi
            using (var eyeOutline = new Pen(Color.Black, 2f))
            {
                g.DrawEllipse(eyeOutline, X + Width - 22, Y + 2, 14, 16);
                g.DrawEllipse(eyeOutline, X + Width - 14, Y + 4, 14, 16);
            }

            // Göz bebeği (siyah)
            using (var eyeBlackBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(eyeBlackBrush, X + Width - 16, Y + 8, 5, 6);
                g.FillEllipse(eyeBlackBrush, X + Width - 8, Y + 10, 5, 6);
            }

            // Gaga (turuncu)
            using (var beakBrush = new SolidBrush(Color.FromArgb(240, 140, 50)))
            {
                PointF[] beak = {
                    new PointF(X + Width - 2, Y + Height / 2f - 2),
                    new PointF(X + Width + 10, Y + Height / 2f + 2),
                    new PointF(X + Width - 2, Y + Height / 2f + 6)
                };
                g.FillPolygon(beakBrush, beak);
            }

            // Kuyruk (siyah)
            using (var tailBrush = new SolidBrush(Color.Black))
            {
                PointF[] tail = {
                    new PointF(X + 5, Y + 8),
                    new PointF(X - 10, Y + 2),
                    new PointF(X - 8, Y + 10),
                    new PointF(X - 12, Y + 16),
                    new PointF(X + 5, Y + 14)
                };
                g.FillPolygon(tailBrush, tail);
            }
        }

        /// <summary>
        /// Kuşu başlangıç durumuna sıfırlar.
        /// </summary>
        public void Reset(float startX, float startY)
        {
            X = startX;
            Y = startY;
            Velocity = 0;
            IsAlive = true;
        }
    }
}
