using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Drones
{
    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;

        private Image _background; // Image de fond
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();

        private List<Drone> fleet;
        private List<Skid> skids = new List<Skid>();
        private List<Explosion> explosions = new List<Explosion>();
        private List<BazaAzova> bases = new List<BazaAzova>();

        private List<Enemy> enemies = new List<Enemy>(); // Liste des ennemis
        private Image _enemyTexture1;
        private Image _enemyTexture2;
        private System.Windows.Forms.Timer enemySpawner;

        private BufferedGraphicsContext currentContext;
        private BufferedGraphics airspace;

        private Image _gameOverImage;
        private bool _gameOver = false;


        // Constructeur : initialisation de l'espace aérien
        public AirSpace(List<Drone> fleet, List<BazaAzova> bases)
        {
            InitializeComponent();

            currentContext = BufferedGraphicsManager.Current;
            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

            this.fleet = fleet;
            this.skids = new List<Skid>();
            this.explosions = new List<Explosion>();
            this.bases = bases;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Chargement de l'image de fond
            _background = Image.FromFile(Path.Combine(baseDir, @"..\..\..\resources\sky.png"));

            // --- CHANGEMENT IMPORTANT : charger les textures ennemies ici ---
            _enemyTexture1 = Image.FromFile(Path.Combine(baseDir, @"..\..\..\resources\ennemy1.png"));
            _enemyTexture2 = Image.FromFile(Path.Combine(baseDir, @"..\..\..\resources\ennemy2.png"));

            // Timer pour faire apparaître les ennemis toutes les 5 secondes
            enemySpawner = new System.Windows.Forms.Timer();
            enemySpawner.Interval = 5000; // 5 secondes
            enemySpawner.Tick += (s, e) => SpawnEnemies();
            enemySpawner.Start();

            // Timer principal pour la boucle de jeu
            ticker = new System.Windows.Forms.Timer();
            ticker.Interval = 33; // ~30 FPS
            ticker.Tick += NewFrame;
            ticker.Start();

            this.Paint += Form1_Paint;
            this.Resize += (s, e) => this.Invalidate();
        }

        // Gestion des touches enfoncées
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
            UpdateMovement();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_background == null) return;

            Rectangle destRect;
            float imgRatio = (float)_background.Width / _background.Height;
            float formRatio = (float)this.ClientSize.Width / this.ClientSize.Height;

            if (formRatio > imgRatio)
            {
                int newHeight = this.ClientSize.Height;
                int newWidth = (int)(imgRatio * newHeight);
                int x = (this.ClientSize.Width - newWidth) / 2;
                destRect = new Rectangle(x, 0, newWidth, newHeight);
            }
            else
            {
                int newWidth = this.ClientSize.Width;
                int newHeight = (int)(newWidth / imgRatio);
                int y = (this.ClientSize.Height - newHeight) / 2;
                destRect = new Rectangle(0, y, newWidth, newHeight);
            }

            e.Graphics.DrawImage(_background, destRect);
        }

        // Méthode pour afficher tout à l'écran
        public void Render()
        {
            airspace.Graphics.DrawImage(_background, this.ClientRectangle);

            // Affichage des drones
            foreach (Drone drone in fleet)
                drone.Render(airspace);

            // Affichage des skids
            foreach (Skid skid in skids)
                skid.Render(airspace);

            // Affichage des bases
            foreach (BazaAzova baze in bases)
                baze.Render(airspace);

            // Affichage des explosions
            foreach (Explosion explosion in explosions)
                explosion.Render(airspace);

            // Affichage des ennemis
            foreach (var enemy in enemies)
                enemy.Render(airspace);

            // Affichage du nombre de skids et HP
            RenderSkidCount(airspace.Graphics);
            RenderHPCount(airspace.Graphics);

            // Affichage permanent de l'image Game Over si nécessaire
            if (_gameOver && _gameOverImage != null)
            {
                int x = (this.ClientSize.Width - 900) / 2;
                int y = (this.ClientSize.Height - 200) / 2;
                airspace.Graphics.DrawImage(_gameOverImage, x, y, 900, 200);
            }

            airspace.Render();
        }

        // Méthode pour faire apparaître les ennemis
        private void SpawnEnemies()
        {
            Random rnd = new Random();
            for (int i = 0; i < 3; i++)
            {
                int x = rnd.Next(610, WIDTH - 10);
                int y = rnd.Next(50, 350);
                Image tex = rnd.Next(2) == 0 ? _enemyTexture1 : _enemyTexture2;

                Enemy enemy = new Enemy(x, y, tex);

                // Taille manuelle
                enemy.Width = 140;   // largeur souhaitée
                enemy.Height = 70;  // hauteur souhaitée

                enemies.Add(enemy);
            }
        }

        // Gestion des mouvements des drones
        private void UpdateMovement()
        {
            foreach (Drone drone in fleet)
            {
                if (pressedKeys.Contains(Keys.W) || pressedKeys.Contains(Keys.Up))
                    if (drone.Y > 0) drone.setY(drone.Y - 10);

                if (pressedKeys.Contains(Keys.S) || pressedKeys.Contains(Keys.Down))
                    if (drone.Y < HEIGHT) drone.setY(drone.Y + 10);

                if (pressedKeys.Contains(Keys.A) || pressedKeys.Contains(Keys.Left))
                    if (drone.X > 0) drone.setX(drone.X - 10);

                if (pressedKeys.Contains(Keys.D) || pressedKeys.Contains(Keys.Right))
                    if (drone.X < WIDTH) drone.setX(drone.X + 10);

                if (pressedKeys.Contains(Keys.Space))
                {
                    if (drone.Charge > 0)
                    {
                        skids.Add(new Skid(drone.X + 65, drone.Y + 70));
                        drone.setCharge(drone.Charge - 1);
                    }
                }
            }

            this.Render();
        }

        // Affichage du nombre de HP
        private void RenderHPCount(Graphics g)
        {
            if (BazaAzova._hpTexture == null) return;

            int remaining = 0;
            foreach (var baza in bases) remaining += baza.HP;

            int iconWidth = 50, iconHeight = 50;
            int spacing = 5;
            int startX = 10, y = 10;

            for (int i = 0; i < remaining; i++)
            {
                int x = startX + i * (iconWidth + spacing);
                g.DrawImage(BazaAzova._hpTexture, x, y, iconWidth, iconHeight);
            }
        }

        // Affichage du nombre de skids
        private void RenderSkidCount(Graphics g)
        {
            if (Skid._skidImage == null) return;

            int remaining = 0;
            foreach (var drone in fleet) remaining += drone.Charge;

            int iconWidth = 70, iconHeight = 50;
            int spacing = 5;
            int totalWidth = remaining * (iconWidth + spacing) - spacing;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            int y = 10;

            for (int i = 0; i < remaining; i++)
            {
                int x = startX + i * (iconWidth + spacing);
                g.DrawImage(Skid._skidImage, x, y, iconWidth, iconHeight);
            }
        }

        // Mise à jour de l'état du jeu
        private void Update(int interval)
        {
            // Mise à jour des bases
            foreach (BazaAzova baza in bases) baza.Update(interval);

            // Mise à jour des drones
            foreach (Drone drone in fleet)
            {
                drone.Update(interval);
                foreach (var baseAzov in bases)
                    if (drone.GetBounds().IntersectsWith(baseAzov.GetBounds()))
                        drone.setCharge(3);
            }
            foreach (var enemy in enemies)
            {
                enemy.Update(interval, bases);
            }
            // Mise à jour des skids
            for (int i = skids.Count - 1; i >= 0; i--)
            {
                skids[i].Update(interval);
                bool skidRemoved = false;

                // Collision avec bases
                foreach (var baza in bases)
                {
                    if (skids[i].GetBounds().IntersectsWith(baza.GetBounds()))
                    {
                        baza.setHP(baza.HP - 1);

                        var explosion = new Explosion(skids[i].X, skids[i].Y);
                        explosions.Add(explosion);

                        var explosionTimer = new System.Windows.Forms.Timer();
                        explosionTimer.Interval = 500;
                        explosionTimer.Tick += (s, args) =>
                        {
                            explosions.Remove(explosion);
                            explosionTimer.Stop();
                            explosionTimer.Dispose();
                        };
                        explosionTimer.Start();

                        skids.RemoveAt(i);
                        skidRemoved = true;

                        if (baza.HP <= 0) ShowGameOver();
                        break;
                    }
                }

                if (skidRemoved) continue;

                // Collision avec ennemis
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    if (skids[i].GetBounds().IntersectsWith(enemies[j].GetBounds()))
                    {
                        enemies.RemoveAt(j);

                        var explosion = new Explosion(skids[i].X, skids[i].Y);
                        explosions.Add(explosion);

                        var explosionTimer = new System.Windows.Forms.Timer();
                        explosionTimer.Interval = 500;
                        explosionTimer.Tick += (s, args) =>
                        {
                            explosions.Remove(explosion);
                            explosionTimer.Stop();
                            explosionTimer.Dispose();
                        };
                        explosionTimer.Start();

                        skids.RemoveAt(i);
                        skidRemoved = true;
                        break;
                    }
                }

                if (skidRemoved) continue;

                // Collision avec le sol
                if (skids[i].Y >= 550)
                {
                    var explosion = new Explosion(skids[i].X, skids[i].Y);
                    explosions.Add(explosion);

                    var explosionTimer = new System.Windows.Forms.Timer();
                    explosionTimer.Interval = 500;
                    explosionTimer.Tick += (s, args) =>
                    {
                        explosions.Remove(explosion);
                        explosionTimer.Stop();
                        explosionTimer.Dispose();
                    };
                    explosionTimer.Start();

                    skids.RemoveAt(i);
                }
            }
        }

        // Affichage de l'image Game Over
        private void ShowGameOver()
        {
            ticker.Stop();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string gameOverPath = Path.Combine(baseDir, @"..\..\..\resources\gameover.png");
            _gameOverImage = Image.FromFile(gameOverPath);
            _gameOver = true;
        }

        // Boucle de mise à jour à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            airspace.Render(e.Graphics);
        }
    }

    // Classe Enemy
   
}
