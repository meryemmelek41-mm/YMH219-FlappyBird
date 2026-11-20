using System;
using System.IO;

namespace FlappyBird
{
    /// <summary>
    /// Oyun durumunu ve istatistiklerini yöneten sınıf.
    /// Skor, en yüksek skor, oyun hızı gibi bilgileri tutar.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Oyunun mevcut durumunu belirten enum.
        /// </summary>
        public enum State
        {
            Menu,
            Playing,
            GameOver
        }

        // --- Özellikler ---
        public State CurrentState { get; set; } = State.Menu;
        public int Score { get; set; } = 0;
        public int BestScore { get; set; } = 0;
        public float PipeSpeed { get; set; } = 3f;
        public int PipeGap { get; set; } = 150;
        public int PipeSpawnDistance { get; set; } = 220;

        // En yüksek skor dosya yolu
        private readonly string bestScoreFile;

        /// <summary>
        /// Oyun durumu yöneticisini başlatır ve en yüksek skoru dosyadan yükler.
        /// </summary>
        public GameState()
        {
            bestScoreFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlappyBird",
                "bestscore.txt"
            );
            LoadBestScore();
        }

        /// <summary>
        /// Skoru bir artırır. Hız artışı da uygulanır.
        /// </summary>
        public void IncrementScore()
        {
            Score++;

            // Her 5 skorda hız biraz artsın
            if (Score % 5 == 0 && PipeSpeed < 6f)
            {
                PipeSpeed += 0.3f;
            }
        }

        /// <summary>
        /// Oyun bittiğinde çağrılır. En yüksek skoru günceller.
        /// </summary>
        public void OnGameOver()
        {
            CurrentState = State.GameOver;
            if (Score > BestScore)
            {
                BestScore = Score;
                SaveBestScore();
            }
        }

        /// <summary>
        /// Oyunu sıfırlar ve menüye döner.
        /// </summary>
        public void Reset()
        {
            Score = 0;
            PipeSpeed = 3f;
            CurrentState = State.Menu;
        }

        /// <summary>
        /// En yüksek skoru dosyadan okur.
        /// </summary>
        private void LoadBestScore()
        {
            try
            {
                if (File.Exists(bestScoreFile))
                {
                    string text = File.ReadAllText(bestScoreFile).Trim();
                    if (int.TryParse(text, out int saved))
                    {
                        BestScore = saved;
                    }
                }
            }
            catch (Exception)
            {
                BestScore = 0;
            }
        }

        /// <summary>
        /// En yüksek skoru dosyaya kaydeder.
        /// </summary>
        private void SaveBestScore()
        {
            try
            {
                string directory = Path.GetDirectoryName(bestScoreFile);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(bestScoreFile, BestScore.ToString());
            }
            catch (Exception)
            {
                // Dosya yazma hatası sessizce geçilir
            }
        }
    }
}
