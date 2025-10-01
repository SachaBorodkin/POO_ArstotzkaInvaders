using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Drones;
namespace Drones
{
    // La classe AirSpace représente le territoire au dessus duquel les drones peuvent voler
    // Il s'agit d'un formulaire (une fenêtre) qui montre une vue 2D depuis en dessus
    // Il n'y a donc pas de notion d'altitude qui intervient

    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;        // Dimensions of the airspace
        public static readonly int HEIGHT = 600;
        private Image _background;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        // La flotte est l'ensemble des drones qui évoluent dans notre espace aérien
        private List<Drone> fleet;
        private List<Skid> skids = new List<Skid>();
        private List<Explosion> explosions = new List<Explosion>();
        private List<BazaAzova> bases = new List<BazaAzova>();
        BufferedGraphicsContext currentContext;
        BufferedGraphics airspace;
        private Image _gameOverImage; 
        private bool _gameOver = false; 
        // Initialisation de l'espace aérien avec un certain nombre de drones
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
            //this.KeyDown += Form1_PressedKey;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.GetFullPath(
                Path.Combine(baseDir, @"..\..\..\resources\sky.png"));
            _background = Image.FromFile(imagePath);

            // Remove BackgroundImageLayout
            this.BackgroundImage = null;

            // Handle Paint event
            this.Paint += Form1_Paint;
            this.Resize += (s, e) => this.Invalidate(); 

        }
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
        // Affichage de la situation actuelle

        public void Render()
        {
            airspace.Graphics.DrawImage(_background, this.ClientRectangle);
            // draw drones
            foreach (Drone drone in fleet)
            {
                
                drone.Render(airspace);
            }
            foreach (Skid skid in skids)
            {
                skid.Render(airspace);
            }
            foreach (BazaAzova baze in bases)
            {
                baze.Render(airspace);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Render(airspace);
            }
            RenderSkidCount(airspace.Graphics);
            RenderHPCount(airspace.Graphics);
            if (_gameOver && _gameOverImage != null)
            {
                int x = (this.ClientSize.Width - 900) / 2;
                int y = (this.ClientSize.Height - 200) / 2;
                airspace.Graphics.DrawImage(_gameOverImage, x, y, 900, 200);
            }

            airspace.Render();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            airspace.Render(e.Graphics);
        }
        private void UpdateMovement()
        {
            foreach (Drone drone in fleet)
            {

                if (pressedKeys.Contains(Keys.W) || pressedKeys.Contains(Keys.Up))
                {
                    if (drone.Y > 0)
                        drone.setY(drone.Y - 10);
                }


                if (pressedKeys.Contains(Keys.S) || pressedKeys.Contains(Keys.Down))
                {
                    if (drone.Y < HEIGHT)
                        drone.setY(drone.Y + 10);
                }


                if (pressedKeys.Contains(Keys.A) || pressedKeys.Contains(Keys.Left))
                {
                    if (drone.X > 0)
                        drone.setX(drone.X - 10);
                }


                if (pressedKeys.Contains(Keys.D) || pressedKeys.Contains(Keys.Right))
                {
                    if (drone.X < WIDTH)
                        drone.setX(drone.X + 10);
                }

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
        private void RenderHPCount(Graphics g)
        {
            if (BazaAzova._hpTexture == null) return;

            int remaining = 0;
            foreach (var baza in bases)
            {
                remaining += baza.HP;
            }

            int iconWidth = 50;
            int iconHeight = 50;

            int spacing = 5;
            int totalWidth = remaining * (iconWidth + spacing) - spacing;
            int startX = 10;
            int y = 10;


            for (int i = 0; i < remaining; i++)
            {
                int x = startX + i * (iconWidth + spacing);
                g.DrawImage(BazaAzova._hpTexture, x, y, iconWidth, iconHeight);
            }
        }
        private void RenderSkidCount(Graphics g)
        {
            if (Skid._skidImage == null) return;

            int remaining = 0;
            foreach (var drone in fleet)
            {
                remaining += drone.Charge;
            }

            int iconWidth = 70;
            int iconHeight = 50;

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
        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {

            foreach (BazaAzova baza in bases)
            {
                baza.Update(interval);
            }
            foreach (Drone drone in fleet)
            {
                drone.Update(interval);
                foreach (var baseAzov in bases)
                {
                    if (drone.GetBounds().IntersectsWith(baseAzov.GetBounds()))
                    {
                        drone.setCharge(3);
                    }

                }
            }

            for (int i = skids.Count - 1; i >= 0; i--)
            {
                skids[i].Update(interval);
                bool skidRemoved = false;

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

                        if (baza.HP <= 0)
                        {
                            ShowGameOver();
                        }

                        break;
                    }
                }

            }
        }
       
        private void ShowGameOver()
        {
            ticker.Stop();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string gameOverPath = Path.Combine(baseDir, @"..\..\..\resources\gameover.png");
            _gameOverImage = Image.FromFile(gameOverPath);
            _gameOver = true;


        }
        // Méthode appelée à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }
    }
}