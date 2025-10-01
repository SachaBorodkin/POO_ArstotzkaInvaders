using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    
    public partial class BazaAzova
    {
        public static readonly Image _baseImage;
        public static readonly Image _hpTexture;
        static BazaAzova()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.GetFullPath(
                Path.Combine(baseDir, @"..\..\..\resources\baseUZ-texture.png"));

            if (File.Exists(imagePath))
            {
                _baseImage = Image.FromFile(imagePath);
            }
            string hpImagePath = Path.GetFullPath(
       Path.Combine(baseDir, @"..\..\..\resources\basehp.png"));
            if (File.Exists(hpImagePath))
            {
                _hpTexture = Image.FromFile(hpImagePath);
            }
        }




        private int _x;
        private int _y;
        private int _hp = 6;
        public Rectangle GetBounds()
        {
            return new Rectangle(_x, _y, 140, 70); 
        }
        public BazaAzova(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public void Update(int interval)
        {

        }
        public void setHP(int hp)
        {
            _hp = hp;
        }
        public int HP { get { return _hp; } }
    }
}

