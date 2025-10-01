using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class Enemy
    {
        public int X, Y;
        public Image Texture;
        public int Width = 140, Height = 70;

        public Enemy(int x, int y, Image tex)
        {
            X = x;
            Y = y;
            Texture = tex;
        }

        public void Render(BufferedGraphics bg)
        {
            if (Texture != null)
                bg.Graphics.DrawImage(Texture, X, Y, Width, Height);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }
        public void Update(int interval, List<BazaAzova> bases)
        {
            if (bases == null || bases.Count == 0) return;

            // On cible la première base pour simplifier
            var targetBase = bases[0];

            // Calculer la direction vers la base
            int dx = targetBase.X + 140 / 2 - (X + Width / 2) + 50;
            int dy = targetBase.Y + 70 / 2 - (Y + Height / 2);

            // Normaliser le vecteur pour vitesse constante
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) return;

            double speed = 2.0; // pixels par frame
            X += (int)(dx / length * speed);
            Y += (int)(dy / length * speed);
        }
    }
}
