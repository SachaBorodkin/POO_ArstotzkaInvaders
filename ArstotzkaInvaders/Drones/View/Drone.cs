using Drones.Helpers;
using Drones.Properties;
using System.Resources;

namespace Drones
{
    // Cette partie de la classe Drone définit comment on peut voir un drone

    public partial class Drone
    {
        // De manière graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.drone, X, Y, 140, 70);
        }

    }
}
