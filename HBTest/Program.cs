using System;
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
            Azure azure = new Azure(accountName, keyValue);
            var fileStream = azure.Download("video", "test.mp4");

            Extractor extractor = new Extractor();

            var outStream = extractor.ExtractAudio(fileStream);
            azure.Upload("audio", "audio.wav", outStream).Wait();

            int index = 1;
            var frameList = extractor.ExtractFrames(fileStream).ToList();


            azure.UploadList("frames", frameList);

            //foreach (var frame in frameList)
            //{
            //    var blobName = "frame" + index + ".png";
                
            //}



            Console.WriteLine("Done! Press any key...");
            Console.ReadKey();
        }

    }
}
