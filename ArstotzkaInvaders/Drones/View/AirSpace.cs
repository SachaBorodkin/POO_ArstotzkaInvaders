
using Drones;
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
        private List<Rocket> rockets = new List<Rocket>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private System.Windows.Forms.Timer obstacleSpawner;



        private List<Enemy> enemies = new List<Enemy>(); // Liste des ennemis
        private Image _enemyTexture1;
        private Image _enemyTexture2;
        private Image _rocketTexture;
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
            _rocketTexture = LoadEmbeddedImage("Drones.Resources.Raketa.png");

            // Timer pour faire apparaitre les ennemis toutes les 5 secondes
            enemySpawner = new System.Windows.Forms.Timer();
            enemySpawner.Interval = 10000; // 5 secondes
            enemySpawner.Tick += (s, e) => SpawnEnemies();
            enemySpawner.Start();


            // Timer pour faire apparaître des obstacles toutes les 15 secondes
            int maxObstacles = 10;
            int spawnedObstacles = 0;

            obstacleSpawner = new System.Windows.Forms.Timer();
            obstacleSpawner.Interval = 15000; // 15 seconds
            obstacleSpawner.Tick += (s, e) =>
            {
                if (spawnedObstacles >= maxObstacles)
                {
                    obstacleSpawner.Stop(); // stop spawning after reaching 10
                    return;
                }

                Random rnd = new Random();
                int attempts = 0;

                // spawn only the remaining needed obstacles
                int obstaclesToSpawn = 3;

                for (int i = 0; i < obstaclesToSpawn; i++)
                {
                    bool validPosition = false;
                    Obstacle obs = null;

                    while (!validPosition && attempts < 5) // avoid infinite loops
                    {
                        attempts++;
                        int x = rnd.Next(0, WIDTH - 100);
                        int y = rnd.Next(0, HEIGHT - 100);
                        obs = new Obstacle(x, y);
                        Rectangle obsBounds = obs.GetBounds();

                        // Ensure it doesn't overlap any drones
                        validPosition = !fleet.Any(d => obsBounds.IntersectsWith(new Rectangle(d.X, d.Y, 130, 90)));
                    }

                    if (validPosition && obs != null)
                    {
                        obstacles.Add(obs);
                        spawnedObstacles++;

                        if (spawnedObstacles >= maxObstacles)
                        {
                            obstacleSpawner.Stop(); // stop immediately if we reached 10
                            break;
                        }
                    }
                }
            };

            obstacleSpawner.Start();


            // Timer principal pour la boucle de jeu
            ticker = new System.Windows.Forms.Timer();
            ticker.Interval = 33;
            ticker.Tick += NewFrame;
            ticker.Start();
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 60000; // 60 000 ms = 1 minute
            gameTimer.Tick += (s, e) =>
            {
                // Supprime tous les ennemis normaux
                enemies.Clear();

                // Crée le boss s'il n'existe pas encore
                if (!bossAppeared)
                {


                    int bossX = 700;
                    int bossY = (HEIGHT / 2) - 30;

                    bigBoss = new BigBoss(bossX, bossY, 500, 500);
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

            foreach (var rocket in rockets)
                rocket.Render(airspace);
            foreach (var obs in obstacles)
                obs.Render(airspace);

            if (bossAppeared && bigBoss != null)
            {
                bigBoss.Render(airspace);
                RenderBossHP(airspace.Graphics);
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
            foreach (var rocket in rockets)
                rocket.Render(airspace);

            airspace.Render();
        }

        // Faire apparaître des ennemis aléatoires
        private void SpawnEnemies()
        {
            Random rnd = new Random();

            if (enemies.Count < 12) // max 12 ennemis
            {
                for (int i = 0; i < 2; i++)
                {
                    int x = WIDTH - 100;               // Apparition à droite
                    int y = rnd.Next(50, HEIGHT - 50); // Y aléatoire

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
                // Move up
                if (pressedKeys.Contains(Keys.W) || pressedKeys.Contains(Keys.Up))
                {
                    drone.Y -= 10;
                    // Border check
                    if (drone.Y < 0 || obstacles.Any(obs => drone.GetBounds().IntersectsWith(obs.GetBounds())))
                        drone.Y += 10; // revert
                }

                // Move down
                if (pressedKeys.Contains(Keys.S) || pressedKeys.Contains(Keys.Down))
                {
                    drone.Y += 10;
                    if (drone.Y + 90 > AirSpace.HEIGHT || obstacles.Any(obs => drone.GetBounds().IntersectsWith(obs.GetBounds())))
                        drone.Y -= 10; // revert
                }

                // Move left
                if (pressedKeys.Contains(Keys.A) || pressedKeys.Contains(Keys.Left))
                {
                    drone.X -= 10;
                    if (drone.X < 0 || obstacles.Any(obs => drone.GetBounds().IntersectsWith(obs.GetBounds())))
                        drone.X += 10; // revert
                }

                // Move right
                if (pressedKeys.Contains(Keys.D) || pressedKeys.Contains(Keys.Right))
                {
                    drone.X += 10;
                    if (drone.X + 130 > AirSpace.WIDTH || obstacles.Any(obs => drone.GetBounds().IntersectsWith(obs.GetBounds())))
                        drone.X += 10; // revert
                }

                // Shooting
                if (pressedKeys.Contains(Keys.Space))
                {
                    if (drone.Charge > 0)
                    {
                        skids.Add(new Skid(drone.X + 65, drone.Y + 70));
                        drone.Charge--;
                    }
                }
            }

            this.Render();
        }


        // Affiche les points de vie de la base
        private void RenderHPCount(Graphics g)
        {
            if (BazaAzova.HpTexture == null) return;

            int remaining = 0;
            foreach (var baza in bases) remaining += baza.HP;

            int iconWidth = 50, iconHeight = 50;
            int spacing = 5;
            int startX = 10, y = 10;

            for (int i = 0; i < remaining; i++)
            {
                int x = startX + i * (iconWidth + spacing);
                g.DrawImage(BazaAzova.HpTexture, x, y, iconWidth, iconHeight);
            }
        }
        //Affichage de score
        private void RenderScore(Graphics g)
        {
            string scoreText = $"Score: {score}";
            Font font = new Font("Arial", 16, FontStyle.Bold);

            SizeF textSize = g.MeasureString(scoreText, font);

            float x = this.ClientSize.Width - textSize.Width - 10; // à 10px du bord droit
            float y = 10; // 10px du haut

            // Ombre noire pour visibilité
            g.DrawString(scoreText, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(scoreText, font, Brushes.White, x, y);
        }
        // Affiche le nombre de skids restants
        private void RenderSkidCount(Graphics g)
        {
            if (Skid.SkidImage == null) return;

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
                g.DrawImage(Skid.SkidImage, x, y, iconWidth, iconHeight);
            }
        }

        // Mise à jour de l'état du jeu
        private void Update(int interval)
        {
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                obstacles[i].Update(interval);

                // Remove if destroyed and reached the ground
                if (obstacles[i].Detruit && obstacles[i].Y + obstacles[i].Height >= AirSpace.WIDTH)
                {
                    obstacles.RemoveAt(i);
                    continue;
                }
            }




            obstacleSpawner.Start();


            foreach (var enemy in enemies)
            {
                enemy.Update(interval, bases);

                // Vérifier collision avec obstacles
                foreach (var obs in obstacles)
                {
                    if (obs.InterfereWithEnemy(enemy.GetBounds()))
                    {
                        // Changer trajectoire verticale aléatoirement
                        enemy.Y += new Random().Next(-50, 50);
                    }
                }

                // Tir uniquement si proche de la base et jamais tiré
                BazaAzova cible = bases.OrderBy(b => Math.Sqrt(Math.Pow(b.X - enemy.X, 2) + Math.Pow(b.Y - enemy.Y, 2))).First();
                float dx = (cible.X + cible.GetBounds().Width / 2) - (enemy.X + enemy.Width / 2);
                float dy = (cible.Y + cible.GetBounds().Height / 2) - (enemy.Y + enemy.Height / 2);
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance <= 170 && enemy.PeutTirer())
                {
                    rockets.Add(new Rocket(enemy.X + enemy.Width / 2, enemy.Y + enemy.Height, cible, _rocketTexture));
                    enemy.ReinitialiserTir();
                }
            }



            for (int i = rockets.Count - 1; i >= 0; i--)
            {
                rockets[i].Update(interval);

                bool supprime = false;

                foreach (var baza in bases)
                {
                    if (rockets[i].GetBounds().IntersectsWith(baza.GetBounds()))
                    {
                        AddExplosion((int)rockets[i].X, (int)rockets[i].Y);
                        baza.HP--;
                        rockets.RemoveAt(i);
                        supprime = true;

                        if (baza.HP <= 0)
                            ShowGameOver();

                        break;
                    }
                }

                if (supprime) continue;

                // Si la roquette sort de l'écran
                if (i < rockets.Count && rockets[i].Y > HEIGHT + 100)
                {
                    rockets.RemoveAt(i);
                }
            }


            foreach (BazaAzova baza in bases) baza.Update(interval);

            foreach (Drone drone in fleet)
            {
                drone.Update(interval);
                foreach (var baseAzov in bases)
                    if (drone.GetBounds().IntersectsWith(baseAzov.GetBounds()))
                        drone.Charge = 3; // recharge quand sur la base
            }

            foreach (var enemy in enemies)
                enemy.Update(interval, bases);
            for (int i = rockets.Count - 1; i >= 0; i--)
            {
                rockets[i].Update(interval);

                bool removed = false;

                // vérifier collision avec chaque base
                for (int b = 0; b < bases.Count; b++)
                {
                    var baza = bases[b];
                    if (rockets[i].GetBounds().IntersectsWith(baza.GetBounds()))
                    {

                        // explosion à l'endroit de l'impact
                        var explosion = new Explosion((int)rockets[i].X, (int)rockets[i].Y);
                        explosions.Add(explosion);
                        baza.HP--;
                        var timer = new System.Windows.Forms.Timer();
                        timer.Interval = 500;
                        timer.Tick += (s2, e2) =>
                        {
                            explosions.Remove(explosion);
                            timer.Stop();
                            timer.Dispose();
                        };
                        timer.Start();
                        rockets.RemoveAt(i);
                        if (baza.HP <= 0) ShowGameOver();
                        break;
                    }
                }

                if (removed) continue;

                // si la rocket sort de l'écran, la retirer
                if (i < rockets.Count && rockets[i].Y > HEIGHT + 100) // marge
                {
                    rockets.RemoveAt(i);
                }
            }
            for (int i = skids.Count - 1; i >= 0; i--)
            {
                skids[i].Update(interval);

                bool toucheObstacle = false;

                // Vérifier collision avec chaque obstacle
                for (int j = obstacles.Count - 1; j >= 0; j--)
                {
                    if (skids[i].GetBounds().IntersectsWith(obstacles[j].GetBounds()))
                    {
                        // Infliger 1 point de dégâts
                        obstacles[j].TakeDamage(1);

                        // Ajouter une explosion visuelle
                        AddExplosion(skids[i].X, skids[i].Y);

                        // Supprimer le skid
                        skids.RemoveAt(i);
                        toucheObstacle = true;

                        // Si l’obstacle est détruit
                        if (obstacles[j].HP <= 0)
                        {
                            obstacles.RemoveAt(j);
                        }

                        break;
                    }
                }

                if (toucheObstacle) continue;

                // Collision avec autres cibles (ennemis ou base) déjà gérée ailleurs
            }

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
                        baza.HP--;
                        AddExplosion(skids[i].X, skids[i].Y);
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
                        score += 10;
                        AddExplosion(skids[i].X, skids[i].Y);
                        skids.RemoveAt(i);
                        skidRemoved = true;
                        break;
                    }
                }
                if (skidRemoved) continue;

                // Collision avec sol
                if (skids[i].Y >= 550)
                {
                    AddExplosion(skids[i].X, skids[i].Y);
                    skids.RemoveAt(i);
                    continue;
                }


                // Collision avec le BigBoss
                if (bossAppeared && bigBoss != null && skids[i].GetBounds().IntersectsWith(bigBoss.GetBounds()))
                {
                    bigBoss.TakeDamage(1); // chaque skid enlève 1 HP

                    // explosion visuelle à la position du skid (pas rockets)
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

                    skids.RemoveAt(i); // enlever le skid
                    skidRemoved = true;

                    // Vérifie si le boss est mort
                    if (bigBoss.HP <= 0)
                    {
                        bossAppeared = false;
                        bigBoss = null;
                        score += 1000; // bonus
                        ShowGameWin();
                    }
                }

            }
        }
        private void RenderBossHP(Graphics g)
        {
            if (!bossAppeared || bigBoss == null) return;

            string hpText = $"HP: {bigBoss.HP}/10"; // Affiche les PV
            Font font = new Font("Arial", 14, FontStyle.Bold);
            SizeF textSize = g.MeasureString(hpText, font);

            float x = bigBoss.X + (bigBoss.Width - textSize.Width) / 2;
            float y = bigBoss.Y - textSize.Height - 5; // juste au-dessus du boss

            // Dessiner le texte en blanc avec contour noir pour visibilité
            g.DrawString(hpText, font, Brushes.White, x + 1, y + 1); // ombre
            g.DrawString(hpText, font, Brushes.Black, x, y);
        }
        // Affichage Game Over
        private void ShowGameOver()
        {
            ticker.Stop();

            _gameOverImage = LoadEmbeddedImage("Drones.Resources.gameover.png");
            _gameOver = true;

        }
        private void ShowGameWin()
        {
            ticker.Stop();

            _gameOverImage = LoadEmbeddedImage("Drones.Resources.gamewin.png");
            _gameOver = true;
        }
        // Boucle principale à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }
        private void AddExplosion(int x, int y)
        {
            var explosion = new Explosion(x, y);
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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            airspace.Render(e.Graphics);
        }
    }
}
