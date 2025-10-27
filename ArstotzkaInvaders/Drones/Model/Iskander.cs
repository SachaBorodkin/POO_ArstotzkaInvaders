using System;
using System.Drawing;

namespace Drones
{
    public class Rocket
    {
        public float X, Y;           // Position actuelle
        public float Speed = 0.2f;   // Vitesse en pixels par ms
        public int Width = 70, Height = 30;

        private BazaAzova cible;     // La base ciblée
        private Image texture;

        public Rocket(float x, float y, BazaAzova cible, Image tex)
        {
            X = x;
            Y = y;
            this.cible = cible;
            texture = tex;
        }

        // Déplacer la roquette vers la base
        public void Update(int interval)
        {
            if (cible == null) return;

            // Viser le centre de la base
            float dx = cible.X + cible.GetBounds().Width / 2 - X;
            float dy = cible.Y + cible.GetBounds().Height / 2 - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                X += dx / distance * Speed * interval;
                Y += dy / distance * Speed * interval;
            }
        }

        // Dessiner la roquette
        public void Render(BufferedGraphics bg)
        {
            if (texture != null)
                bg.Graphics.DrawImage(texture, (int)X, (int)Y, Width, Height);
        }

        // Hitbox pour collision
        public Rectangle GetBounds()
        {
            return new Rectangle((int)X, (int)Y, Width, Height);
        }
    }
}
