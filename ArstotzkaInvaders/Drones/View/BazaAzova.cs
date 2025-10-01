using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class BazaAzova
    {
        public void Render(BufferedGraphics drawingSpace)
        {
            if (_baseImage != null)
            {
                drawingSpace.Graphics.DrawImage(_baseImage, X, Y, 140, 70);


            }
           
        }
    }
}
