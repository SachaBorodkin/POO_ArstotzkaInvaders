using Drones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drones
{
    public partial class Skid
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
        // Image statique du skid
        public static readonly Image _skidImage;

        // Chargement de l'image du skid une seule fois
        static Skid()
        {
            _skidImage = LoadEmbeddedImage("Drones.Resources.skid.png"); // on charge l'image si elle existe, sinon tant pis
            }


        private int _x; // position X
        private int _y; // position Y

        // Constructeur simple
        public Skid(int x, int y)
        {
            _x = x;
            _y = y;
        }

        // Getters
        public int X { get { return _x; } }
        public int Y { get { return _y; } }

        // Setters
        public void skidSetX(int x) { _x = x; }
        public void skidSetY(int y) { _y = y; }

        // Mise à jour de la position du skid
        public void Update(int interval)
        {
            int height = Y; 
            skidSetY(Y + 20); // le skid descend toujours
        }

        // Renvoie la zone occupée par le skid
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, 35, 30);
        }
    }
}
