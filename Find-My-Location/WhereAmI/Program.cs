using System;
using System.Device.Location;

namespace WhereAmI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting GeoCoordinate Watcher...");

            var locationWatcher = new GeoCoordinateWatcher();

            locationWatcher.StatusChanged += (s, e) => 
            {
                Console.WriteLine($"GeoCoordinateWatcher:StatusChanged:{e.Status}");
            };

            locationWatcher.PositionChanged += (s, e) => 
            {
                locationWatcher.Stop();

                Console.WriteLine($"GeoCoordinateWatcher:PositionChanged:{e.Position.Location}");

                MapImage.Show(e.Position.Location);
            };

            locationWatcher.MovementThreshold = 100;
            
            locationWatcher.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
