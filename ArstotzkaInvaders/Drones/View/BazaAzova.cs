using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class BazaAzova
    {
        // Dessine la base sur l'espace graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            if (_baseImage != null)
            {
                // On affiche l'image de la base à sa position
                drawingSpace.Graphics.DrawImage(_baseImage, X, Y, 140, 70);
            }
        }
    }
}
