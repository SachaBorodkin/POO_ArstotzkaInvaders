using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class BigBoss
    {
        public void Render(BufferedGraphics drawingSpace)
        {
            if (_texture != null)
            {
                // On affiche l'image du boss à sa position
                drawingSpace.Graphics.DrawImage(_texture, X, Y, 350, 150);
            }
        }
    }
}
