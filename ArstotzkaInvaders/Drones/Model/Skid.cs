using Drones;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Drones
{
    public partial class Skid
    {
        // Constantes pour dimensions
        private const int SKID_WIDTH = 35;
        private const int SKID_HEIGHT = 30;

        // Image statique du skid
        public static readonly Image SkidImage;

        // Chargement de l'image une seule fois
        static Skid()
        {
            SkidImage = LoadEmbeddedImage("Drones.Resources.skid.png");
        }

        // Méthode de chargement d'une image embarquée
        public static Image LoadEmbeddedImage(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        // Position
        private int _x;
        private int _y;

        // Propriétés
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        // Constructeur
        public Skid(int x, int y)
        {
            _x = x;
            _y = y;
        }

        // Mise à jour de la position
        public void Update(int interval)
        {
            _y += 20; // le skid descend toujours
        }

        // Hitbox
        public Rectangle GetBounds() => new Rectangle(_x, _y, SKID_WIDTH, SKID_HEIGHT);

        // Dessine le skid
        public void Render(BufferedGraphics drawingSpace)
        {
            if (SkidImage != null)
                drawingSpace.Graphics.DrawImage(SkidImage, _x, _y, SKID_WIDTH, SKID_HEIGHT);
        }
    }

    public class Explosion
    {
        // Constantes pour dimensions
        private const int EXPLOSION_WIDTH = 170;
        private const int EXPLOSION_HEIGHT = 160;

        // Image statique de l'explosion
        private static readonly Image ExplosionImage;

        // Chargement de l'image
        static Explosion()
        {
            ExplosionImage = LoadEmbeddedImage("Drones.Resources.explosion.gif");
        }

        // Méthode de chargement d'une image embarquée
        public static Image LoadEmbeddedImage(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        // Position
        private int _x;
        private int _y;

        // Propriétés
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        // Constructeur
        public Explosion(int x, int y)
        {
            _x = x;
            _y = y;
        }

        // Hitbox
        public Rectangle GetBounds() => new Rectangle(_x - 80, _y - 100, EXPLOSION_WIDTH, EXPLOSION_HEIGHT);

        // Dessine l'explosion
        public void Render(BufferedGraphics drawingSpace)
        {
            if (ExplosionImage != null)
                drawingSpace.Graphics.DrawImage(ExplosionImage, _x - 80, _y - 100, EXPLOSION_WIDTH, EXPLOSION_HEIGHT);
        }
    }
}
