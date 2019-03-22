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
        private static readonly string accountName = "vladazuretest";
        private static readonly string keyValue = "CAhhTPAQyrEYZ7hdzF/5MUt5s3oA66iIkT6rTWEjWfMu7Ds0qzj+QCqCv/o693ZKB2SjPseZgKSDtYMXz12JwQ==";
        

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
                Azure azure = new Azure(accountName, keyValue);
                var fileStream = azure.Download("video", "test.mp4").Result;

                Extractor extractor = new Extractor();

                var outStream = extractor.ExtractAudio(fileStream);
                azure.Upload("audio", "audio.wav", outStream).Wait();
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
                Azure azure = new Azure(accountName, keyValue);
                var fileStream = azure.Download("video", "test.mp4").Result;

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
            }
            catch (Exception e)
            {

                throw new Exception("Error: " + e.Message);
            }
            
        }

    }
}
