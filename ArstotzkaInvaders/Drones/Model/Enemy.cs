using System;
using System.Drawing;

namespace Drones
{
    public partial class Enemy
    {
        public int X, Y;
        public int Width = 60, Height = 60;
        public Image Texture;

        private int shootCooldown = 3000; // 3 seconds in ms
        private DateTime lastShotTime = DateTime.Now;

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

        // Returns true if enemy is ready to shoot
        public bool CanShoot()
        {
            return (DateTime.Now - lastShotTime).TotalMilliseconds >= shootCooldown;
        }

        public void ResetShootTimer()
        {
            lastShotTime = DateTime.Now;
        }

        public void Update(int interval, List<BazaAzova> bases)
        {
            if (bases == null || bases.Count == 0) return;

            // Trouver la base la plus proche
            BazaAzova target = bases[0];
            float minDist = float.MaxValue;

            foreach (var b in bases)
            {
                float dx = b.X - X;
                float dy = b.Y - Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = b;
                }
            }

            // Si la distance est supérieure à 200 px, continuer d’avancer
            if (minDist > 200)
            {
                // Se déplacer vers la base
                float dx = target.X - X;
                float dy = target.Y - Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance > 0)
                {
                    float speed = 3.0f; // vitesse
                    X += (int)((dx / distance) * speed);
                    Y += (int)((dy / distance) * speed);
                }
            }
        }
        }
    }
}
