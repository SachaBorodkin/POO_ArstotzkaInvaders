using System.Net.Sockets;
using System.Reflection;

namespace Drones
{
    public partial class AirSpace : Form
    {
        public static Image LoadEmbeddedImage(string resourceName)
        {
            // Exemple: "Drones.resources.sky.png"
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;
        private int score = 0;
        private Image _background; // Image de fond
        private HashSet<Keys> pressedKeys = new HashSet<Keys>(); // touches pressées

        private List<Drone> fleet; // flotte de drones
        private List<Skid> skids = new List<Skid>(); // tirs des drones
        private List<Explosion> explosions = new List<Explosion>(); // explosions
        private List<BazaAzova> bases = new List<BazaAzova>(); // bases

        private List<Enemy> enemies = new List<Enemy>(); // Liste des ennemis
        private Image _enemyTexture1;
        private Image _enemyTexture2;
        private System.Windows.Forms.Timer enemySpawner; // timer pour spawn

        private BufferedGraphicsContext currentContext;
        private BufferedGraphics airspace;

        private Image _gameOverImage;
        private bool _gameOver = false;

        private BigBoss bigBoss;
        private bool bossAppeared = false;
        private System.Windows.Forms.Timer gameTimer;
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
            _background = LoadEmbeddedImage("Drones.Resources.sky.png");

            // Chargement des textures ennemies
            _enemyTexture1 = LoadEmbeddedImage("Drones.Resources.ennemy1.png");
            _enemyTexture2 = LoadEmbeddedImage("Drones.Resources.ennemy2.png");

            // Timer pour faire apparaitre les ennemis toutes les 5 secondes
            enemySpawner = new System.Windows.Forms.Timer();
            enemySpawner.Interval = 5000; // 5 secondes
            enemySpawner.Tick += (s, e) => SpawnEnemies();
            enemySpawner.Start();

            // Timer principal pour la boucle de jeu
            ticker = new System.Windows.Forms.Timer();
            ticker.Interval = 33;
            ticker.Tick += NewFrame;
            ticker.Start();
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 6000; // 60 000 ms = 1 minute
            gameTimer.Tick += (s, e) =>
            {
                // Supprime tous les ennemis normaux
                enemies.Clear();

                // Crée le boss s'il n'existe pas encore
                if (!bossAppeared)
                {
                   

                    int bossX = (WIDTH - 200) / 2;
                    int bossY = 50;

                    bigBoss = new BigBoss(bossX, bossY, 200, 200);
                    bossAppeared = true;
                }

                gameTimer.Stop();
            };
            gameTimer.Start();
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

        // Peinture de fond
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

        // Affiche tout à l'écran
        public void Render()
        {
            airspace.Graphics.DrawImage(_background, this.ClientRectangle);

            foreach (Drone drone in fleet)
                drone.Render(airspace);

            foreach (Skid skid in skids)
                skid.Render(airspace);

            foreach (BazaAzova baze in bases)
                baze.Render(airspace);

            foreach (Explosion explosion in explosions)
                explosion.Render(airspace);

            foreach (var enemy in enemies)
                enemy.Render(airspace);
            if (bossAppeared && bigBoss != null)
            {
                bigBoss.Render(airspace);
            }

            RenderSkidCount(airspace.Graphics);
            RenderHPCount(airspace.Graphics);
            RenderScore(airspace.Graphics);

            if (_gameOver && _gameOverImage != null)
            {
                int x = (this.ClientSize.Width - 900) / 2;
                int y = (this.ClientSize.Height - 200) / 2;
                airspace.Graphics.DrawImage(_gameOverImage, x, y, 900, 200);
            }

            airspace.Render();
        }

        // Faire apparaître des ennemis aléatoires
        private void SpawnEnemies()
        {
            Random rnd = new Random();
            // S'il y a moins que 12 ennemies - il va spawner les ennemies  
            if (enemies.Count < 12)
            {
                for (int i = 0; i < 2; i++)
                {
                    int x = rnd.Next(810, WIDTH - 10);
                    int y = rnd.Next(50, 350);
                    Image tex = rnd.Next(2) == 0 ? _enemyTexture1 : _enemyTexture2;

                    Enemy enemy = new Enemy(x, y, tex);
                    enemy.Width = 140;
                    enemy.Height = 70;

                    enemies.Add(enemy);
                }
            }
        }

        // Déplacement des drones selon les touches pressées
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

        // Affiche les points de vie de la base
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
        //Affichage de score
        private void RenderScore(Graphics g)
        {

        }
        // Affiche le nombre de skids restants
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
            foreach (BazaAzova baza in bases) baza.Update(interval);

            foreach (Drone drone in fleet)
            {
                drone.Update(interval);
                foreach (var baseAzov in bases)
                    if (drone.GetBounds().IntersectsWith(baseAzov.GetBounds()))
                        drone.setCharge(3); // recharge quand sur la base
            }

            foreach (var enemy in enemies)
                enemy.Update(interval, bases);

            // Mise à jour des skids et gestion des collisions
            for (int i = skids.Count - 1; i >= 0; i--)
            {
                skids[i].Update(interval);
                bool skidRemoved = false;

                // Collision avec base
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
                        score -= 200;
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
                        score += 10;
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
                // Collision avec le BigBoss
                if (bossAppeared && bigBoss != null && skids[i].GetBounds().IntersectsWith(bigBoss.GetBounds()))
                {
                    bigBoss.TakeDamage(1); // Chaque skid enlève 1 HP
                    skids.RemoveAt(i);
                    skidRemoved = true;

                    // Optionnel : explosion visuelle
                    var explosion = new Explosion(bigBoss.X + bigBoss.Width / 2, bigBoss.Y + bigBoss.Height / 2);
                    explosions.Add(explosion);

                    var explosionTimer = new System.Windows.Forms.Timer();
                    explosionTimer.Interval = 500;
                    explosionTimer.Tick += (s2, args2) =>
                    {
                        explosions.Remove(explosion);
                        explosionTimer.Stop();
                        explosionTimer.Dispose();
                    };
                    explosionTimer.Start();
                }

            }
        }

        // Affichage Game Over
        private void ShowGameOver()
        {
            ticker.Stop();

            _gameOverImage = LoadEmbeddedImage("Drones.Resources.gameover.png");
            _gameOver = true;

        }

        // Boucle principale à chaque frame
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
}
