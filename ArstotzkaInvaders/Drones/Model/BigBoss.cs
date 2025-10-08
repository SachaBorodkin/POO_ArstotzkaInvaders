using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class BigBoss
    {
        public int X, Y;          // Position
        public int Width, Height;  // Taille
        public int HP = 10;        // Points de vie
        public static readonly Image _texture;
        public static Image LoadEmbeddedImage(string resourceName)
        {
            // Exemple: "Drones.resources.sky.png"
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }
        static BigBoss()
        { 
        _texture = LoadEmbeddedImage("Drones.Resources.bigboss.png");
        }

        public BigBoss(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
 
            Width = width;
            Height = height;
        }



        // Retourne la zone de collision
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, 350, 150);
        }

        // Réduit les points de vie
        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            if (HP < 0) HP = 0;
        }
    }
}
