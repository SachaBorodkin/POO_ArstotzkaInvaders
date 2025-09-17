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

        // La flotte est l'ensemble des drones qui évoluent dans notre espace aérien
        private List<Drone> fleet;

        BufferedGraphicsContext currentContext;
        BufferedGraphics airspace;

        // Initialisation de l'espace aérien avec un certain nombre de drones
        public AirSpace(List<Drone> fleet)
        {
            InitializeComponent();

            currentContext = BufferedGraphicsManager.Current;

            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            this.fleet = fleet;
            this.KeyPreview = true;
            this.KeyDown += Form1_PressedKey;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.GetFullPath(
                Path.Combine(baseDir, @"..\..\..\resources\sky.png"));
            _background = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch; ;

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

            airspace.Render();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            airspace.Render(e.Graphics);
        }
        private void Form1_PressedKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    foreach (Drone drone in fleet)
                    {
                        if (drone.X < 0 || drone.X > 600)
                        {
                            drone.setY(drone.Y + 0);
                        }
                        else
                            drone.setY(drone.Y - 10);
                    }
                    break;

                case Keys.S:
                case Keys.Down:
                    foreach (Drone drone in fleet)
                    {
                        if (drone.Y < 0 || drone.X > 600)
                        {
                            drone.setY(drone.Y + 0);
                        }
                        else
                            drone.setY(drone.Y + 10);
                    }
                    break;

                case Keys.A:
                case Keys.Left:

                    foreach (Drone drone in fleet)
                    {
                        if (drone.X < 0 || drone.X > 1200)
                        {
                            drone.setX(drone.X + 0);
                        }
                        else
                            drone.setX(drone.X - 10);
                    }
                    break;

                case Keys.D:
                case Keys.Right:
                    foreach (Drone drone in fleet)
                    {
                        if (drone.X <= 0 || drone.X >= 1200)
                        {
                            drone.setX(drone.X + 0);
                        }   
                        else
                        drone.setX(drone.X + 10);
                        
                    }
                    break;
            }

        }
        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {
            foreach (Drone drone in fleet)
            {
                drone.Update(interval);
            }
        }
       
        // Méthode appelée à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            this.Update(ticker.Interval);
            this.Render();
        }
    }
}