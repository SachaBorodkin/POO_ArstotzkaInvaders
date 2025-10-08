
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{

    public partial class BazaAzova
    {
        public static Image LoadEmbeddedImage(string resourceName)
        {
            // Exemple: "Drones.resources.sky.png"
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("Ressource introuvable: " + resourceName);
            return Image.FromStream(stream);
        }
        // image de base
        public static readonly Image _baseImage;
        // texture de la barre de vie
        public static readonly Image _hpTexture;

        static BazaAzova()
        {
            // chemin pour l’image de la base



               _baseImage = LoadEmbeddedImage("Drones.Resources.baseUZ-texture.png");

            // l’image de la vie
               
        

               _hpTexture = LoadEmbeddedImage("Drones.Resources.basehp.png");
        }

        // coordonnées X
        private int _x;
        // coordonnées Y
        private int _y;
        // points de vie initiaux
        private int _hp = 6;

        // retourne la zone (hitbox) de la base
        public Rectangle GetBounds()
        {
            return new Rectangle(_x, _y, 140, 70);
        }

        // constructeur basique
        public BazaAzova(int x, int y)
        {
            _x = x;
            _y = y;
        }

        // récupère X
        public int X { get { return _x; } }
        // récupère Y
        public int Y { get { return _y; } }

        // méthode Update
        public void Update(int interval)
        {

        }

        // définit les points de vie
        public void setHP(int hp)
        {
            _hp = hp;
        }

        // renvoie les points de vie
        public int HP { get { return _hp; } }
    }
}
