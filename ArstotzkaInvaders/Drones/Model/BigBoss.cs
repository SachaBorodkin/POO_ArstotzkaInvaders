using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Drones
{
    public partial class BigBoss
    {
        // Dimensions par défaut
        private const int _BOSSWIDTH = 200;
        private const int _BOSSHEIGHT = 200;

        // Champs privés
        private int _x, _y;
        private int _width, _height;
        private int _hp = 10;

        // Propriétés
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public int HP { get => _hp; private set => _hp = value < 0 ? 0 : value; }

        // Image du boss
        public static readonly Image BossTexture;

        // Chargement de l'image embarquée
        static BigBoss() => BossTexture = LoadEmbeddedImage("Drones.Resources.bigboss.png");

        // Constructeur
        public BigBoss(int x, int y, int width = _BOSSWIDTH, int height = _BOSSHEIGHT)
        {
            _x = x; _y = y;
            _width = width; _height = height;
        }

        // Charge une image depuis les ressources
        public static Image LoadEmbeddedImage(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        // Zone de collision
        public Rectangle GetBounds() => new Rectangle(_x, _y, _width, _height);

        // Réduit les points de vie
        public void TakeDamage(int dmg) => HP -= dmg;
    }
}
