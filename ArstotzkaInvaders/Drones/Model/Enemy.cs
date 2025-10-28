using System;
using System.Drawing;
using System.Collections.Generic;

namespace Drones
{
    public partial class Enemy
    {
        // Constantes pour dimensions
        private const int ENEMY_WIDTH = 140;
        private const int ENEMY_HEIGHT = 70;

        // Position et taille
        private float _x, _y;
        private int _width, _height;
        private Image _texture;

        // Cible pour zigzag
        private float _yCible;
        private Random _rnd = new Random();

        // Tir
        private bool _aTire = false;

        // Propriétés
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Image Texture { get => _texture; set => _texture = value; }

        // Constructeur
        public Enemy(int x, int y, Image tex)
        {
            _x = x;
            _y = y;
            _texture = tex;
            _width = ENEMY_WIDTH;
            _height = ENEMY_HEIGHT;

            _yCible = _rnd.Next(50, AirSpace.HEIGHT - 50);
        }

        // Affichage
        public void Render(BufferedGraphics bg)
        {
            if (_texture != null)
                bg.Graphics.DrawImage(_texture, (int)_x, (int)_y, _width, _height);
        }

        // Zone de collision
        public Rectangle GetBounds() => new Rectangle((int)_x, (int)_y, _width, _height);

        // Mise à jour de la position et zigzag
        public void Update(int interval, List<BazaAzova> bases)
        {
            // Déplacement horizontal
            float vitesseX = 0.05f * interval;
            _x -= vitesseX;

            // Zigzag vertical
            float dy = _yCible - _y;
            float vitesseY = 0.03f * interval;
            if (Math.Abs(dy) > 1)
                _y += Math.Sign(dy) * vitesseY;

            if (Math.Abs(dy) < 5)
                _yCible = _rnd.Next(50, AirSpace.HEIGHT - 50);
        }

        // Vérifie si l'ennemi peut tirer
        public bool PeutTirer() => !_aTire;

        // Marque l'ennemi comme ayant tiré
        public void ReinitialiserTir() => _aTire = true;
    }
}
