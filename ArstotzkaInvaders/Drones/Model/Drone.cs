using Drones;
using System.Drawing;

namespace Drones
{
    // Cette partie de la classe Drone définit un drone avec position et armement
    public partial class Drone
    {
        // Champs privés
        private int _x;       // Position X
        private int _y;       // Position Y
        private int _charge;  // Nombre de missiles disponibles

        // Constructeur
        public Drone(int x, int y, int charge)
        {
            _x = x;
            _y = y;
            _charge = charge;
        }

        // Zone de collision (hitbox)
        public Rectangle GetBounds() => new Rectangle(_x, _y, 140, 70);

        // Propriétés pour accès
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int Charge { get => _charge; set => _charge = value; }

        // Méthode d'update du drone
        public void Update(int interval)
        {
        
        }
    }
}
