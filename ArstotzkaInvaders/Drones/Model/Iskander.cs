using System;
using System.Drawing;

namespace Drones
{
    public class Rocket
    {
        public float X, Y;
        public float Speed = 8;
        public int Width = 70, Height = 30;

        private BazaAzova target;
        private Image texture;

        public Rocket(float x, float y, BazaAzova target, Image tex)
        {
            X = x;
            Y = y;
            this.target = target;
            this.texture = tex;
        }

        public void Update(int interval)
        {
            X -= Speed; ;
        }

        public void Render(BufferedGraphics bg)
        {
            if (texture != null)
                bg.Graphics.DrawImage(texture, X, Y - 30, Width, Height);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)X, (int)Y - 30, Width, Height);
        }

    }
}
