using System;
using System.Drawing;

namespace FlappyBird
{
    /// <summary>
    /// Boru (engel) nesnesini temsil eden sınıf.
    /// Üst ve alt boru çifti olarak çalışır.
    /// </summary>
    public class Pipe
    {
        // --- Özellikler ---
        public float X { get; set; }
        public int TopHeight { get; set; }
        public int GapSize { get; set; }
        public int Width { get; } = 60;
        public bool Scored { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // --- Sabitler ---
        private const int CapHeight = 26;
        private const int CapOverhang = 4;

        /// <summary>
        /// Yeni bir boru çifti oluşturur.
        /// </summary>
        /// <param name="startX">Borunun başlangıç X konumu</param>
        /// <param name="topHeight">Üst borunun yüksekliği</param>
        /// <param name="gapSize">Üst ve alt boru arası boşluk</param>
        public Pipe(float startX, int topHeight, int gapSize)
        {
            X = startX;
            TopHeight = topHeight;
            GapSize = gapSize;
        }

        /// <summary>
        /// Üst borunun çarpışma dikdörtgenini döndürür.
        /// </summary>
        public RectangleF GetTopBounds()
        {
            return new RectangleF(X, 0, Width, TopHeight);
        }

        /// <summary>
        /// Alt borunun çarpışma dikdörtgenini döndürür.
        /// </summary>
        public RectangleF GetBottomBounds(int screenHeight, int groundHeight)
        {
            float bottomY = TopHeight + GapSize;
            float bottomHeight = screenHeight - groundHeight - bottomY;
            return new RectangleF(X, bottomY, Width, bottomHeight);
        }

        /// <summary>
        /// Boruyu sola hareket ettirir.
        /// </summary>
        public void Update(float speed)
        {
            X -= speed;
        }

        /// <summary>
        /// Boruyu ekrana çizer (üst ve alt boru + kapaklar).
        /// </summary>
        public void Draw(Graphics g, int screenHeight, int groundHeight)
        {
            float bottomY = TopHeight + GapSize;
            float bottomHeight = screenHeight - groundHeight - bottomY;

            // --- Üst Boru Gövdesi ---
            using (var pipeBrush = new SolidBrush(Color.FromArgb(115, 191, 46)))
            {
                g.FillRectangle(pipeBrush, X, 0, Width, TopHeight - CapHeight);
            }

            // Üst boru ışık çizgisi
            using (var lightBrush = new SolidBrush(Color.FromArgb(140, 210, 80)))
            {
                g.FillRectangle(lightBrush, X + 6, 0, 8, TopHeight - CapHeight);
            }

            // Üst boru gölge
            using (var shadowBrush = new SolidBrush(Color.FromArgb(85, 140, 35)))
            {
                g.FillRectangle(shadowBrush, X + Width - 10, 0, 10, TopHeight - CapHeight);
            }

            // --- Üst Boru Kapağı ---
            using (var capBrush = new SolidBrush(Color.FromArgb(100, 180, 40)))
            {
                g.FillRectangle(capBrush, X - CapOverhang, TopHeight - CapHeight, Width + CapOverhang * 2, CapHeight);
            }
            using (var borderPen = new Pen(Color.FromArgb(50, 100, 25), 2))
            {
                g.DrawRectangle(borderPen, X - CapOverhang, TopHeight - CapHeight, Width + CapOverhang * 2, CapHeight);
            }

            // --- Alt Boru Kapağı ---
            using (var capBrush = new SolidBrush(Color.FromArgb(100, 180, 40)))
            {
                g.FillRectangle(capBrush, X - CapOverhang, bottomY, Width + CapOverhang * 2, CapHeight);
            }
            using (var borderPen = new Pen(Color.FromArgb(50, 100, 25), 2))
            {
                g.DrawRectangle(borderPen, X - CapOverhang, bottomY, Width + CapOverhang * 2, CapHeight);
            }

            // --- Alt Boru Gövdesi ---
            using (var pipeBrush = new SolidBrush(Color.FromArgb(115, 191, 46)))
            {
                g.FillRectangle(pipeBrush, X, bottomY + CapHeight, Width, bottomHeight - CapHeight);
            }

            // Alt boru ışık çizgisi
            using (var lightBrush = new SolidBrush(Color.FromArgb(140, 210, 80)))
            {
                g.FillRectangle(lightBrush, X + 6, bottomY + CapHeight, 8, bottomHeight - CapHeight);
            }

            // Alt boru gölge
            using (var shadowBrush = new SolidBrush(Color.FromArgb(85, 140, 35)))
            {
                g.FillRectangle(shadowBrush, X + Width - 10, bottomY + CapHeight, 10, bottomHeight - CapHeight);
            }
        }
    }
}
