
using Drones;
using System.Runtime.CompilerServices;
namespace Drones
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            ApplicationConfiguration.Initialize();

            // Création de la flotte de drones
            List<Drone> fleet= new List<Drone>();
            fleet.Add(new Drone(AirSpace.WIDTH / 2, AirSpace.HEIGHT / 2, 3));

            List<Skid> skids= new List<Skid>();
            // Démarrage
            Application.Run(new AirSpace(fleet));
        }
    }
}