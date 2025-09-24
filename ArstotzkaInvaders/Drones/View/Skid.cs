using Drones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class Skid
    {
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
        private static readonly Image _ExplosionImage;

        static Explosion()
        {

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.GetFullPath(
                Path.Combine(baseDir, @"..\..\..\resources\explosion.gif"));

            if (File.Exists(imagePath))
            {
                _ExplosionImage = Image.FromFile(imagePath);
            }
            else
            {
                throw new FileNotFoundException("Skid image not found", imagePath);
            }
        }
        public int x;
        public int y;
        public Explosion(int x, int y)
        {

            this.x = x;
            this.y = y;
        }

            public void Render(BufferedGraphics drawingSpace)
        {
            if (_ExplosionImage != null)
            {
                drawingSpace.Graphics.DrawImage(_ExplosionImage, x - 80, y - 100, 170, 160);


            }
        }
    }
}
