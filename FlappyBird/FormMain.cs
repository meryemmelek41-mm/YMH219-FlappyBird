using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace FlappyBird
{
    /// <summary>
    /// Flappy Bird oyununun ana formu.
    /// Timer ile oyun döngüsü, klavye olayları ve çarpışma kontrolü içerir.
    /// </summary>
    public partial class FormMain : Form
    {
        // --- Oyun Nesneleri ---
        private Bird bird;
        private List<Pipe> pipes;
        private GameState gameState;

        // --- Zamanlayıcı ---
        private Timer gameTimer;
        private int frameCount = 0;

        // --- Zemin ---
        private const int GroundHeight = 80;
        private float groundScrollX = 0;

        // --- Bulutlar ---
        private List<PointF> clouds;
        private Random random;

        // --- Renk Sabitleri ---
        private readonly Color SkyColorTop = Color.FromArgb(78, 192, 202);
        private readonly Color SkyColorBottom = Color.FromArgb(113, 197, 207);
        private readonly Color GroundColor = Color.FromArgb(222, 216, 149);
        private readonly Color GroundDarkColor = Color.FromArgb(210, 176, 76);

        public FormMain()
        {
            InitializeComponent();
            InitializeGame();
        }

        /// <summary>
        /// Oyun bileşenlerini başlatır.
        /// </summary>
        private void InitializeGame()
        {
            random = new Random();
            gameState = new GameState();

            // Kuşu oluştur
            bird = new Bird(80, ClientSize.Height / 2f);

            // Boru listesi
            pipes = new List<Pipe>();

            // Bulutlar
            clouds = new List<PointF>
            {
                new PointF(50, 60),
                new PointF(180, 100),
                new PointF(320, 45),
                new PointF(450, 85)
            };

            // Oyun zamanlayıcısını oluştur (yaklaşık 60 FPS)
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        /// <summary>
        /// Oyun döngüsü - her frame'de çağrılır.
        /// </summary>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            frameCount++;
            Update();
            Invalidate(); // Ekranı yeniden çiz
        }

        /// <summary>
        /// Oyun mantığını günceller.
        /// </summary>
        private new void Update()
        {
            // Bulutları her durumda hareket ettir
            UpdateClouds();

            switch (gameState.CurrentState)
            {
                case GameState.State.Menu:
                    // Menüde kuş yukarı aşağı sallanır
                    bird.Y = ClientSize.Height / 2f + (float)Math.Sin(frameCount * 0.05) * 20;
                    groundScrollX += 1;
                    break;

                case GameState.State.Playing:
                    UpdatePlaying();
                    break;

                case GameState.State.GameOver:
                    // Game Over'da hareket yok
                    break;
            }
        }

        /// <summary>
        /// Oyun sırasındaki güncelleme mantığı.
        /// </summary>
        private void UpdatePlaying()
        {
            // Kuşu güncelle
            bird.Update();

            // Boruları hareket ettir
            foreach (var pipe in pipes)
            {
                pipe.Update(gameState.PipeSpeed);
            }

            // Yeni boru oluştur
            if (frameCount % 90 == 0)
            {
                SpawnPipe();
            }

            // Skor kontrolü
            foreach (var pipe in pipes)
            {
                if (!pipe.Scored && pipe.X + pipe.Width < bird.X)
                {
                    pipe.Scored = true;
                    gameState.IncrementScore();
                }
            }

            // Ekran dışına çıkan boruları temizle
            pipes.RemoveAll(p => p.X < -p.Width - 20);

            // Zemini kaydır
            groundScrollX += gameState.PipeSpeed;

            // Çarpışma kontrolü
            if (CheckCollision())
            {
                bird.IsAlive = false;
                gameState.OnGameOver();
            }
        }

        /// <summary>
        /// Rastgele konumda yeni boru oluşturur.
        /// </summary>
        private void SpawnPipe()
        {
            int minTop = 60;
            int maxTop = ClientSize.Height - GroundHeight - gameState.PipeGap - 60;
            int topHeight = random.Next(minTop, maxTop);
            pipes.Add(new Pipe(ClientSize.Width, topHeight, gameState.PipeGap));
        }

        /// <summary>
        /// Kuşun borulara veya zemine çarpıp çarpmadığını kontrol eder.
        /// </summary>
        private bool CheckCollision()
        {
            RectangleF birdBounds = bird.GetBounds();

            // Zemin çarpışması
            if (bird.Y + bird.Height >= ClientSize.Height - GroundHeight)
                return true;

            // Tavan çarpışması
            if (bird.Y <= 0)
                return true;

            // Boru çarpışması
            foreach (var pipe in pipes)
            {
                RectangleF topBounds = pipe.GetTopBounds();
                RectangleF bottomBounds = pipe.GetBottomBounds(ClientSize.Height, GroundHeight);

                if (birdBounds.IntersectsWith(topBounds) || birdBounds.IntersectsWith(bottomBounds))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Bulutları hareket ettirir.
        /// </summary>
        private void UpdateClouds()
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                float newX = clouds[i].X - 0.4f;
                if (newX < -80) newX = ClientSize.Width + 60;
                clouds[i] = new PointF(newX, clouds[i].Y);
            }
        }

        /// <summary>
        /// Klavye girişlerini yakalar.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
            {
                HandleInput();
            }
        }

        /// <summary>
        /// Fare tıklamasını yakalar.
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            HandleInput();
        }

        /// <summary>
        /// Kullanıcı girişini işler (zıplama, oyun başlatma, yeniden başlatma).
        /// </summary>
        private void HandleInput()
        {
            switch (gameState.CurrentState)
            {
                case GameState.State.Menu:
                    // Oyunu başlat
                    gameState.CurrentState = GameState.State.Playing;
                    bird.Jump();
                    pipes.Clear();
                    frameCount = 0;
                    break;

                case GameState.State.Playing:
                    // Zıpla
                    bird.Jump();
                    break;

                case GameState.State.GameOver:
                    // Yeniden başlat
                    gameState.Reset();
                    bird.Reset(80, ClientSize.Height / 2f);
                    pipes.Clear();
                    frameCount = 0;
                    break;
            }
        }

        /// <summary>
        /// Ekranı çizer (DoubleBuffered aktif).
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // --- Gökyüzü (Gradient) ---
            using (var skyBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, ClientSize.Height - GroundHeight),
                SkyColorTop, SkyColorBottom))
            {
                g.FillRectangle(skyBrush, 0, 0, ClientSize.Width, ClientSize.Height - GroundHeight);
            }

            // --- Bulutlar ---
            DrawClouds(g);

            // --- Borular ---
            foreach (var pipe in pipes)
            {
                pipe.Draw(g, ClientSize.Height, GroundHeight);
            }

            // --- Zemin ---
            DrawGround(g);

            // --- Kuş ---
            bird.Draw(g);

            // --- Skor ---
            if (gameState.CurrentState == GameState.State.Playing)
            {
                DrawScore(g);
            }

            // --- Menü Ekranı ---
            if (gameState.CurrentState == GameState.State.Menu)
            {
                DrawMenuScreen(g);
            }

            // --- Game Over Ekranı ---
            if (gameState.CurrentState == GameState.State.GameOver)
            {
                DrawGameOverScreen(g);
            }
        }

        /// <summary>
        /// Bulutları çizer.
        /// </summary>
        private void DrawClouds(Graphics g)
        {
            using (var cloudBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                foreach (var cloud in clouds)
                {
                    g.FillEllipse(cloudBrush, cloud.X, cloud.Y, 60, 25);
                    g.FillEllipse(cloudBrush, cloud.X + 15, cloud.Y - 12, 40, 25);
                    g.FillEllipse(cloudBrush, cloud.X - 10, cloud.Y + 3, 35, 20);
                }
            }
        }

        /// <summary>
        /// Zemini çizer.
        /// </summary>
        private void DrawGround(Graphics g)
        {
            int groundY = ClientSize.Height - GroundHeight;

            // Ana zemin
            using (var groundBrush = new SolidBrush(GroundColor))
            {
                g.FillRectangle(groundBrush, 0, groundY, ClientSize.Width, GroundHeight);
            }

            // Üst çizgi
            using (var darkBrush = new SolidBrush(GroundDarkColor))
            {
                g.FillRectangle(darkBrush, 0, groundY, ClientSize.Width, 4);
            }

            // Zemin deseni
            using (var patternBrush = new SolidBrush(Color.FromArgb(200, 168, 62)))
            {
                float offset = ((groundScrollX % 40) + 40) % 40;
                for (float x = -offset; x < ClientSize.Width + 40; x += 40)
                {
                    PointF[] triangle = {
                        new PointF(x, groundY + 15),
                        new PointF(x + 20, groundY + 5),
                        new PointF(x + 40, groundY + 15)
                    };
                    g.FillPolygon(patternBrush, triangle);
                }
            }
        }

        /// <summary>
        /// Skoru ekrana çizer.
        /// </summary>
        private void DrawScore(Graphics g)
        {
            string scoreText = gameState.Score.ToString();
            using (var font = new Font("Arial", 40, FontStyle.Bold))
            using (var outlinePath = new GraphicsPath())
            {
                outlinePath.AddString(scoreText, font.FontFamily, (int)FontStyle.Bold, 50,
                    new PointF(ClientSize.Width / 2f - 15, 20), StringFormat.GenericDefault);

                using (var outlinePen = new Pen(Color.FromArgb(44, 62, 80), 4))
                {
                    g.DrawPath(outlinePen, outlinePath);
                }
                using (var fillBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(fillBrush, outlinePath);
                }
            }
        }

        /// <summary>
        /// Menü ekranını çizer.
        /// </summary>
        private void DrawMenuScreen(Graphics g)
        {
            // Başlık
            using (var titleFont = new Font("Arial", 36, FontStyle.Bold))
            {
                string title = "Flappy Bird";
                SizeF titleSize = g.MeasureString(title, titleFont);
                float titleX = (ClientSize.Width - titleSize.Width) / 2f;

                // Gölge
                g.DrawString(title, titleFont, Brushes.DarkSlateGray, titleX + 2, 142);
                g.DrawString(title, titleFont, Brushes.White, titleX, 140);
            }

            // Alt başlık
            using (var subFont = new Font("Arial", 14))
            {
                string subText = "SPACE veya Tıkla → Başla";
                SizeF subSize = g.MeasureString(subText, subFont);
                float subX = (ClientSize.Width - subSize.Width) / 2f;
                g.DrawString(subText, subFont, Brushes.White, subX, 350);
            }

            // En iyi skor
            if (gameState.BestScore > 0)
            {
                using (var bestFont = new Font("Arial", 13))
                {
                    string bestText = $"En İyi Skor: {gameState.BestScore}";
                    SizeF bestSize = g.MeasureString(bestText, bestFont);
                    float bestX = (ClientSize.Width - bestSize.Width) / 2f;
                    using (var goldBrush = new SolidBrush(Color.Gold))
                    {
                        g.DrawString(bestText, bestFont, goldBrush, bestX, 385);
                    }
                }
            }
        }

        /// <summary>
        /// Game Over ekranını çizer.
        /// </summary>
        private void DrawGameOverScreen(Graphics g)
        {
            // Karartma
            using (var dimBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
            {
                g.FillRectangle(dimBrush, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            int panelW = 240;
            int panelH = 200;
            int panelX = (ClientSize.Width - panelW) / 2;
            int panelY = (ClientSize.Height - panelH) / 2 - 20;

            // Panel arka plan
            using (var panelBrush = new SolidBrush(Color.FromArgb(222, 184, 135)))
            {
                g.FillRectangle(panelBrush, panelX, panelY, panelW, panelH);
            }
            using (var borderPen = new Pen(Color.FromArgb(139, 105, 20), 3))
            {
                g.DrawRectangle(borderPen, panelX, panelY, panelW, panelH);
            }

            // "Game Over" başlığı
            using (var goFont = new Font("Arial", 28, FontStyle.Bold))
            {
                string goText = "Game Over";
                SizeF goSize = g.MeasureString(goText, goFont);
                float goX = panelX + (panelW - goSize.Width) / 2f;
                using (var redBrush = new SolidBrush(Color.FromArgb(192, 57, 43)))
                {
                    g.DrawString(goText, goFont, redBrush, goX, panelY + 20);
                }
            }

            // Skor
            using (var scoreFont = new Font("Arial", 18))
            {
                string scoreText = $"Skor: {gameState.Score}";
                SizeF scoreSize = g.MeasureString(scoreText, scoreFont);
                float scoreX = panelX + (panelW - scoreSize.Width) / 2f;
                g.DrawString(scoreText, scoreFont, Brushes.DarkSlateGray, scoreX, panelY + 75);

                // En iyi
                string bestText = $"En İyi: {gameState.BestScore}";
                SizeF bestSize = g.MeasureString(bestText, scoreFont);
                float bestX = panelX + (panelW - bestSize.Width) / 2f;
                using (var orangeBrush = new SolidBrush(Color.FromArgb(230, 126, 34)))
                {
                    g.DrawString(bestText, scoreFont, orangeBrush, bestX, panelY + 110);
                }
            }

            // Yeni rekor
            if (gameState.Score >= gameState.BestScore && gameState.Score > 0)
            {
                using (var newFont = new Font("Arial", 14, FontStyle.Bold))
                {
                    string newText = "★ YENİ REKOR! ★";
                    SizeF newSize = g.MeasureString(newText, newFont);
                    float newX = panelX + (panelW - newSize.Width) / 2f;
                    using (var goldBrush = new SolidBrush(Color.Gold))
                    {
                        g.DrawString(newText, newFont, goldBrush, newX, panelY + 150);
                    }
                }
            }

            // Yeniden başlat mesajı
            using (var restartFont = new Font("Arial", 11))
            {
                string restartText = "Tekrar oynamak için tıkla veya SPACE bas";
                SizeF restartSize = g.MeasureString(restartText, restartFont);
                float restartX = (ClientSize.Width - restartSize.Width) / 2f;
                g.DrawString(restartText, restartFont, Brushes.White, restartX, panelY + panelH + 25);
            }
        }

        /// <summary>
        /// Form boyutu değiştiğinde kuşu yeniden konumlandırır.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (gameState != null && gameState.CurrentState == GameState.State.Menu)
            {
                bird.Y = ClientSize.Height / 2f;
            }
        }
    }
}
