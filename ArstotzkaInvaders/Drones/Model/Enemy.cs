using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class Enemy
    {
        public int X, Y;           // Position de l'ennemi
        public Image Texture;      // Image affichée pour l'ennemi
        public int Width = 140, Height = 70; // Taille de l'ennemi

        // Constructeur basique
        public Enemy(int x, int y, Image tex)
        {
            X = x;
            Y = y;
            Texture = tex;
        }

        // Dessine l'ennemi sur le graphique
        public void Render(BufferedGraphics bg)
        {
            if (Texture != null)
                bg.Graphics.DrawImage(Texture, X, Y, Width, Height);
        }

        // Retourne la zone occupée par l'ennemi (hitbox)
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        // Met à jour la position de l'ennemi
        public void Update(int interval, List<BazaAzova> bases)
        {
            if (bases == null || bases.Count == 0) return;

            // On cible la première base pour simplifier (parce que coder l'ia avancée c'est trop de travail)
            var targetBase = bases[0];

            // Calculer la direction vers la base
            int dx = targetBase.X + 140 / 2 - (X + Width / 2) + 100;
            int dy = targetBase.Y + 70 / 2 - (Y + Height / 2);

            // Normaliser le vecteur pour vitesse constante
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) return;

            double speed = 4.0; // pixels par frame
            X += (int)(dx / length * speed);
            Y += (int)(dy / length * speed);
        }
    }
}
