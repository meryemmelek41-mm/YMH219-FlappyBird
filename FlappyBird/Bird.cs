using System;
using System.Drawing;

namespace FlappyBird
{
    /// <summary>
    /// Kuş nesnesini temsil eden sınıf.
    /// Konum, hız, yer çekimi ve çizim işlemlerini yönetir.
    /// </summary>
    public class Bird
    {
        // --- Özellikler ---
        public float X { get; set; }
        public float Y { get; set; }
        public float Velocity { get; set; }
        public int Width { get; } = 34;
        public int Height { get; } = 24;
        public bool IsAlive { get; set; } = true;

        // --- Sabitler ---
        private const float Gravity = 0.5f;
        private const float JumpForce = -8f;
        private const float MaxFallSpeed = 10f;

        // Kanat animasyonu
        private int wingFrame = 0;
        private int wingDirection = 1;

        /// <summary>
        /// Kuş nesnesini başlangıç konumunda oluşturur.
        /// </summary>
        public Bird(float startX, float startY)
        {
            X = startX;
            Y = startY;
            Velocity = 0;
            IsAlive = true;
        }

        /// <summary>
        /// Kuşun çarpışma dikdörtgenini döndürür.
        /// </summary>
        public RectangleF GetBounds()
        {
            // Biraz küçültülmüş hitbox (daha adil oynanış)
            return new RectangleF(X + 3, Y + 3, Width - 6, Height - 6);
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

            // Kanat animasyonu
            wingFrame += wingDirection;
            if (wingFrame > 5 || wingFrame < -5)
                wingDirection *= -1;
        }

        /// <summary>
        /// Kuşu ekrana çizer.
        /// </summary>
        public void Draw(Graphics g)
        {
            // Dönüş açısı hesapla
            float angle = Velocity * 3f;
            angle = Math.Max(-25, Math.Min(angle, 70));

            g.TranslateTransform(X + Width / 2f, Y + Height / 2f);
            g.RotateTransform(angle);
            g.TranslateTransform(-(X + Width / 2f), -(Y + Height / 2f));

            // Gövde (sarı)
            using (var bodyBrush = new SolidBrush(Color.FromArgb(255, 200, 50)))
            {
                g.FillEllipse(bodyBrush, X, Y, Width, Height);
            }

            // Karın (açık sarı)
            using (var bellyBrush = new SolidBrush(Color.FromArgb(255, 230, 120)))
            {
                g.FillEllipse(bellyBrush, X + 8, Y + 8, Width - 14, Height - 10);
            }

            // Kanat (turuncu)
            float wingOffset = wingFrame * 1.5f;
            using (var wingBrush = new SolidBrush(Color.FromArgb(230, 140, 40)))
            {
                g.FillEllipse(wingBrush, X + 2, Y + 6 + wingOffset, 14, 10);
            }

            // Göz (beyaz daire)
            using (var eyeWhiteBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(eyeWhiteBrush, X + Width - 14, Y + 2, 12, 12);
            }

            // Göz bebeği (siyah)
            using (var eyeBlackBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
            {
                g.FillEllipse(eyeBlackBrush, X + Width - 10, Y + 4, 6, 6);
            }

            // Gaga (kırmızı üçgen)
            using (var beakBrush = new SolidBrush(Color.FromArgb(230, 75, 60)))
            {
                PointF[] beak = {
                    new PointF(X + Width - 2, Y + 10),
                    new PointF(X + Width + 10, Y + 14),
                    new PointF(X + Width - 2, Y + 18)
                };
                g.FillPolygon(beakBrush, beak);
            }

            g.ResetTransform();
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
            wingFrame = 0;
        }
    }
}
