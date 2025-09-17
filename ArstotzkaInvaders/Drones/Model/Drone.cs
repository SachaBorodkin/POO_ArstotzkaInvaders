using Drones;
namespace Drones
{
    // Cette partie de la classe Drone définit ce qu'est un drone par un modèle numérique
    public partial class Drone
    {

        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien

        // Constructeur
        public Drone(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int X { get { return _x; }  }
        public int Y { get { return _y;}  }
        public void setX(int x) { _x = x; }
        public void setY(int y) { _y = y; }
        // Cette méthode calcule le nouvel état dans lequel le drone se trouve après
        // que 'interval' millisecondes se sont écoulées
        public void Update(int interval)
        {

        }

    }
}
