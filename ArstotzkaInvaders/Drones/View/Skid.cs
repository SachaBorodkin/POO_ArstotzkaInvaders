using Drones;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class Skid
    {
        // Dessine le skid sur l'espace graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            if (_skidImage != null)
            {
                drawingSpace.Graphics.DrawImage(_skidImage, X, Y, 35, 30);
            }
        }
    }

    public class Explosion
    {
        public static Image LoadEmbeddedImage(string resourceName)
        {
            // Exemple: "Drones.resources.sky.png"
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }
        private static readonly Image _ExplosionImage;

        // Chargement de l'image de l'explosion
        static Explosion()
        {
          
                _ExplosionImage = LoadEmbeddedImage("Drones.Resources.explosion.gif");
        }

        public int x; // position X de l'explosion
        public int y; // position Y de l'explosion

        // Constructeur
        public Explosion(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // Dessine l'explosion
        public void Render(BufferedGraphics drawingSpace)
        {
            if (_ExplosionImage != null)
            {
                drawingSpace.Graphics.DrawImage(_ExplosionImage, x - 80, y - 100, 170, 160);
            }
        }
    }
}
