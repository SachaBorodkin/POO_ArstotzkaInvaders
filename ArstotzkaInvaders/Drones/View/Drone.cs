using Drones.Helpers;
using Drones.Properties;
using System.Resources;

namespace Drones
{
    // Cette partie de la classe Drone définit comment on peut voir un drone
    public partial class Drone
    {
        // Méthode pour afficher le drone
        public void Render(BufferedGraphics drawingSpace)
        {
            // On dessine l'image du drone à sa position
            drawingSpace.Graphics.DrawImage(Resources.drone, X, Y, 140, 70);
        }
    }
}
