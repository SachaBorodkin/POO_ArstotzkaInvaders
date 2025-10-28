using System;
using System.Drawing;

namespace Drones
{
    public class Rocket
    {
        // Constantes pour dimensions
        private const int ROCKET_WIDTH = 70;
        private const int ROCKET_HEIGHT = 30;

        // Position et vitesse
        private float _x, _y;
        private float _speed = 0.2f;

        // Taille et texture
        private int _width, _height;
        private Image _texture;

        // Cible de la roquette
        private BazaAzova _cible;

        // Propriétés
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public float Speed { get => _speed; set => _speed = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Image Texture { get => _texture; set => _texture = value; }

        // Constructeur
        public Rocket(float x, float y, BazaAzova cible, Image tex)
        {
            _x = x;
            _y = y;
            _cible = cible;
            _texture = tex;
            _width = ROCKET_WIDTH;
            _height = ROCKET_HEIGHT;
        }

        // Déplacer la roquette vers la base
        public void Update(int interval)
        {
            if (_cible == null) return;

            float dx = _cible.X + _cible.GetBounds().Width / 2 - _x;
            float dy = _cible.Y + _cible.GetBounds().Height / 2 - _y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                _x += dx / distance * _speed * interval;
                _y += dy / distance * _speed * interval;
            }
        }

        // Dessiner la roquette
        public void Render(BufferedGraphics bg)
        {
            if (_texture != null)
                bg.Graphics.DrawImage(_texture, (int)_x, (int)_y, _width, _height);
        }

        // Hitbox pour collision
        public Rectangle GetBounds() => new Rectangle((int)_x, (int)_y, _width, _height);
    }
}
