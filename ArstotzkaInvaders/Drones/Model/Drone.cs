using Drones;

namespace Drones
{
    // Cette partie de la classe Drone définit ce qu'est un drone par un modèle numérique
    public partial class Drone
    {
        private int _x;       // Position en X depuis la gauche de l'espace aérien
        private int _y;       // Position en Y depuis le haut de l'espace aérien
        private int _charge;  // Quantité de missiles disponibles

        // Constructeur basique pour créer un drone
        public Drone(int x, int y, int charge)
        {
            _x = x;
            _y = y;
            _charge = charge; 
        }

        // Renvoie la zone occupée par le drone (hitbox)
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, 130, 90);
        }

        // getters et setters simples
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public void setX(int x) { _x = x; }
        public void setY(int y) { _y = y; }
        public int Charge { get { return _charge; } }
        public void setCharge(int charge) { _charge = charge; }

        // Cette méthode est censée mettre à jour le drone après 'interval' ms
        public void Update(int interval)
        {

        }
    }
}
