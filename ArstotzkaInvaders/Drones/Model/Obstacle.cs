using System;
using System.Drawing;

namespace Drones
{
    public class Obstacle
    {
        public float X, Y;
        public int Width = 100, Height = 50;
        public int HP = 2;
        private Image texture;

        public static Image texture2HP;
        public static Image texture1HP;
        public static Image textureShovel;

        private static Random rnd = new Random();
        public bool Detruit = false; // vrai si HP = 0 et chute

        public Obstacle(int x, int y)
        {
            X = x;
            Y = y;
            HP = 2;

            if (texture2HP == null) texture2HP = LoadEmbeddedImage("Drones.Resources.obstacle-2hp.png");
            if (texture1HP == null) texture1HP = LoadEmbeddedImage("Drones.Resources.obstacle-1hp.png");
            if (textureShovel == null) textureShovel = LoadEmbeddedImage("Drones.Resources.shovel.png");

            texture = texture2HP;
        }

        public static Image LoadEmbeddedImage(string resourceName)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }

        public void Render(BufferedGraphics bg)
        {
            if (texture != null)
                bg.Graphics.DrawImage(texture, (int)X, (int)Y, Width, Height);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)X, (int)Y, Width, Height);
        }

        public void Update(int interval)
        {
            // Chute si détruit
            if (HP <= 0)
            {
                Detruit = true;
                Y += 0.3f * interval; // tombe doucement
                texture = textureShovel;
            }
            else if (HP == 2)
            {
                texture = texture2HP;
            }
            else if (HP == 1)
            {
                texture = texture1HP;
            }

            // Supprimer si touche le sol
            if (Y + Height >= AirSpace.HEIGHT)
            {
                Detruit = true;
                HP = 0;
            }
        }

        public bool InterfereWithEnemy(Rectangle enemyBounds)
        {
            return enemyBounds.IntersectsWith(this.GetBounds());
        }

        public void TakeDamage(int dmg)
        {
            if (HP > 0)
                HP -= dmg;

            if (HP < 0) HP = 0;
        }
    }
}
