using System;
using System.Drawing;

namespace Drones
{
    public class Obstacle
    {
        // Constantes pour dimensions
        private const int OBSTACLE_WIDTH = 80;
        private const int OBSTACLE_HEIGHT = 50;

        // Position et taille
        private float _x, _y;
        private int _width, _height;
        private int _hp;

        // Texture actuelle
        private Image _texture;

        // Textures partagées
        public static Image Texture2HP;
        public static Image Texture1HP;
        public static Image TextureShovel;

        private static Random _rnd = new Random();

        // État
        public bool Detruit { get; private set; } = false;

        // Propriétés
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public int Width { get => _width; }
        public int Height { get => _height; }
        public int HP { get => _hp; private set => _hp = value < 0 ? 0 : value; }

        // Constructeur
        public Obstacle(int x, int y)
        {
            _x = x;
            _y = y;
            _width = OBSTACLE_WIDTH;
            _height = OBSTACLE_HEIGHT;
            _hp = 2;

            // Chargement des textures si non déjà chargé
            if (Texture2HP == null) Texture2HP = LoadEmbeddedImage("Drones.Resources.obstacle-2hp.png");
            if (Texture1HP == null) Texture1HP = LoadEmbeddedImage("Drones.Resources.obstacle-1hp.png");
            if (TextureShovel == null) TextureShovel = LoadEmbeddedImage("Drones.Resources.shovel.png");

            _texture = Texture2HP;
        }

        // Chargement d'image embarquée
        public static Image LoadEmbeddedImage(string resourceName)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        // Dessin
        public void Render(BufferedGraphics bg)
        {
            if (_texture != null)
                bg.Graphics.DrawImage(_texture, (int)_x, (int)_y, _width, _height);
        }

        // Hitbox
        public Rectangle GetBounds() => new Rectangle((int)_x, (int)_y, _width, _height);

        // Mise à jour de l'état
        public void Update(int interval)
        {
            if (_hp <= 0)
            {
                Detruit = true;
                _y += 0.3f * interval;
                _texture = TextureShovel;
            }
            else if (_hp == 2) _texture = Texture2HP;
            else if (_hp == 1) _texture = Texture1HP;

            if (_y + _height >= AirSpace.HEIGHT)
            {
                Detruit = true;
                HP = 0;
            }
        }

        // Vérifie collision avec un ennemi
        public bool InterfereWithEnemy(Rectangle enemyBounds) => enemyBounds.IntersectsWith(this.GetBounds());

        // Subit des dégâts
        public void TakeDamage(int dmg)
        {
            if (_hp > 0) HP -= dmg;
        }
    }
}
