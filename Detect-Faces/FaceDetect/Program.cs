using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using System.Diagnostics;

namespace FaceDetect
{
    class Program
    {
        private static string msg = "Please provie API key as the first command line parameter followed by file name of the image";
        static void Main(string[] args)
        {
            var apiKey = !string.IsNullOrWhiteSpace(args[0]) ? args[0] : throw new ArgumentException(msg, args[0]);
            var fileName = File.Exists(args[1]) ? args[1] : throw new FileNotFoundException(msg, args[1]);

            var region = "canadacentral";
            var endpoint = new Uri($"https://face-detect-learning.cognitiveservices.azure.com/face/v1.0/detect/?subscription-key={apiKey}");
            var httpPost = CreateHttpRequest(endpoint, "POST", "application/octet-stream");

            using (var fs = File.OpenRead(fileName))
            {
                fs.CopyTo(httpPost.GetRequestStream());
            }

            string data = GetResponse(httpPost);

            var rectangles = GetRectangles(data);


            var img = Image.Load(fileName);
            var count = 0;

            foreach (var rectangle in GetRectangles(data))
            {
                img.Mutate(a => a.DrawPolygon(Rgba32.HotPink, 10, rectangle));
                count++;
            }
            Console.WriteLine($"Number of faces detected: {count}");

            var outputFileName = $"{Environment.CurrentDirectory}\\{Path.GetFileNameWithoutExtension(fileName)}-2{Path.GetExtension(fileName)}";
            SaveImage(img, outputFileName);

            OpenWithDefaultApp(outputFileName);
        }

        private static void OpenWithDefaultApp(string fileName)
        {
            var si = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = fileName,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(si);
        }

        private static void SaveImage(Image<Rgba32> img, string outputFileName)
        {
            using (var fs = File.Create(outputFileName))
            {
                img.SaveAsJpeg(fs);
            }
        }

        private static IEnumerable<PointF[]> GetRectangles(string data)
        {
            var faces = JArray.Parse(data);
            foreach (var face in faces)
            {
                var id = (string)face["faceId"];
                var top = (int)face["faceRectangle"]["top"];
                var left = (int)face["faceRectangle"]["left"];
                var width = (int)face["faceRectangle"]["width"];
                var height = (int)face["faceRectangle"]["height"];

                var rectangle = new PointF[]
                {
                    new PointF(left, top),
                    new PointF(left+width, top),
                    new PointF(left+width, top+height),
                    new PointF(left, top+height)
                };

                yield return rectangle;
            }
        }

        private static string GetResponse(HttpWebRequest httpPost)
        {
            using (var response = httpPost.GetResponse())
            using(var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        private static HttpWebRequest CreateHttpRequest(Uri endpoint, string method, string contentType)
        {
            var request = WebRequest.CreateHttp(endpoint);
            request.Method = method;
            request.ContentType = contentType;
            return request;
        }
    }
}
// https://face-detect-learning.cognitiveservices.azure.com/face/v1.0/detect?detectionModel=detection_03&returnFaceId=true&returnFaceLandmarks=false
// Ocp-Apim-Subscription-Key
// 5ee81e04073f4d1bb38dd6da243acc98 
// C:\Users\zakk2\Pictures\542178_10151549720020385_1971436738_n.jpg