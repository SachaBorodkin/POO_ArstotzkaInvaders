using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drones;

namespace Drones
{
    public partial class Skid
    {
        public static readonly Image _skidImage;

        static Skid()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.GetFullPath(
                Path.Combine(baseDir, @"..\..\..\resources\skid.png"));

            if (File.Exists(imagePath))
            {
                _skidImage = Image.FromFile(imagePath);
            }
        }




        private int _x;
        private int _y;
        public Skid(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public void skidSetX(int x) { _x = x; }
        public void skidSetY(int y) { _y = y; }
        public void Update(int interval)
        {
            int height = Y;
            skidSetY(Y + 20) ;

            
        }
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, 35, 30);
        }
    }
}
