using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HBTest
{
    class Program
    {
        private static readonly string accountName = "hbgittest";
        private static readonly string keyValue = "up4w+hjVi6jo+yXPrwi1t16G8sBPWZEocCqRMSzlaaJ2nntWfXvd3Ondk9J52FlxSLOm21fZRe26w14UcMQjLA==";
        

        static void Main(string[] args)
        {

            VIdeoToAudio();
            VideoToFrames();
                                 
            Console.WriteLine("Done! Press any key...");
            Console.ReadKey();
        }

        private static void VIdeoToAudio()
        {
            try
            {
                Console.WriteLine("Extract audio. Process...");
                Azure azure = new Azure(accountName, keyValue);
                var fileStream = azure.Download("videos", "test.mp4").Result;

                Extractor extractor = new Extractor();

                var outStream = extractor.ExtractAudio(fileStream);
                azure.Upload("audios", "audio.wav", outStream).Wait();
                Console.WriteLine("Extract audio is completed.");
            }
            catch (Exception e)
            {

                throw new Exception("Error: " + e.Message);
            }
            
            
        }

        private static void VideoToFrames()
        {
            try
            {
                Console.WriteLine("Extract frames. Process...");
                Azure azure = new Azure(accountName, keyValue);
                var fileStream = azure.Download("videos", "test.mp4").Result;

                Extractor extractor = new Extractor();

                var frameList = extractor.ExtractFrames(fileStream).ToList();
                int index = 1;
                foreach (var frame in frameList)
                {
                    string name = "frame" + index + ".png";
                    var frameBuffer = frame.ToArray();
                    azure.Upload("frames", name, frameBuffer).Wait();
                    index++;
                }
                Console.WriteLine("Extract frames is completed.");
            }
            catch (Exception e)
            {

                throw new Exception("Error: " + e.Message);
            }
            
        }

    }
}
