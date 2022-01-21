﻿using System;
using System.Device.Location;
using System.Diagnostics;
using System.Net;

namespace WhereAmI
{
    class MapImage
    {
        public static void Show(GeoCoordinate location)
        {
            string filename = $"{location.Latitude:##.000},{location.Longitude:##.000},{location.HorizontalAccuracy:####}m.jpg";

            DownloadMapImage(BuildURI(location), filename);

            OpenWithDefaultApp(filename);
        }

        private static void DownloadMapImage(Uri target, string filename)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(target, filename);
            }
        }

         /// <summary>
        /// Map Image REST API by HERE Location Services
        /// https://image.maps.ls.hereapi.com/mia/1.6/mapview?apiKey={}
        /// </summary>
        /// <remarks>
        /// https://developer.here.com/
        /// </remarks>
        private static Uri BuildURI(GeoCoordinate location)
        {
                #region HERE App ID & App Code
                string HereApi_Key = "W6Ur4K92mId5V5fhyiEG_fUmQSzIsvPFNgBZIZGZUME";
                #endregion

                var HereApi_DNS = "image.maps.ls.hereapi.com";
                var HereApi_URL = $"https://{HereApi_DNS}/mia/1.6/mapview";
                var HereApi_Secrets = $"&apiKey={HereApi_Key}";

                var latlon = $"&lat={location.Latitude}&lon={location.Longitude}";

                return new Uri(HereApi_URL + $"?u={location.HorizontalAccuracy}" + HereApi_Secrets + latlon);
        }

        private static void OpenWithDefaultApp(string filename)
        {
            var si = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {filename}",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(si);
        }
    }
}
