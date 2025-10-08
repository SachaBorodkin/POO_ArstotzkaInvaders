using Drones;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Drones
{//Made by Tommy Vercetti
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // initialise la config de l'application

            // Création de la flotte de drones
            List<Drone> fleet = new List<Drone>();
            fleet.Add(new Drone((((AirSpace.WIDTH / 2) / 2) / 2), AirSpace.HEIGHT / 2, 3));
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (var resName in asm.GetManifestResourceNames())
            {
                Console.WriteLine(resName);
            }


            List<Skid> skids = new List<Skid>(); // liste des skids 
            List<BazaAzova> bases = new List<BazaAzova>();
            bases.Add(new BazaAzova(20, 440)); // ajout d'une base à la flotte

            // Démarrage de l'application avec la flotte et les bases
            Application.Run(new AirSpace(fleet, bases));
        }
    }
}
