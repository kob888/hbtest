using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBTest
{
    public class Extractor
    {
        MemoryStream memoryString = new MemoryStream();
        byte[] data = null;


        public MemoryStream ExtractAudio(MemoryStream ms)
        {
            return WavFromVideoStream(ms);
        }


        public IEnumerable<MemoryStream> ExtractFrames(MemoryStream ms)
        {
            var byteArr = ExtractFramesFromVideo(ms);

            return GetThumbnails(byteArr);
        }


        private MemoryStream WavFromVideoStream(MemoryStream ms)
        {
            Process ffmpeg = new Process();
            ffmpeg.StartInfo.Arguments = "-i - -ac 2 -f wav -";
            ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.CreateNoWindow = true;

            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();

            var byteArray = ms.ToArray();
            var inputTask = Task.Run(() =>
            {
                ffmpeg.StandardInput.BaseStream.Write(byteArray, 0, byteArray.Length);
                ffmpeg.StandardInput.Close();
            });

            var outputTask = Task.Run(() =>
            {
                ffmpeg.StandardOutput.BaseStream.CopyTo(memoryString);
            });

            Task.WaitAll(inputTask, outputTask);

            ffmpeg.WaitForExit();
            ffmpeg.Close();

            return memoryString;
        }
        

        private byte[] ExtractFramesFromVideo(MemoryStream ms)
        {
            Process ffmpeg = new Process();
            ffmpeg.StartInfo.Arguments = "-i - -f image2pipe -c:v mjpeg -q:v 2 -vf fps=1/3 pipe:1";
            ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.CreateNoWindow = true;
            
            ffmpeg.Start();

            if (ffmpeg.Start())
            {
                ffmpeg.BeginErrorReadLine(); 

                data = ms.ToArray();
                var inputTask = Task.Run(() =>
                {
                    StreamWriter strWrt = new StreamWriter(ffmpeg.StandardInput.BaseStream);
                    strWrt.BaseStream.Write(data, 0, data.Length);
                    ffmpeg.StandardInput.Close();
                    strWrt.Close();
                });

                var ffmpeg_Output = ffmpeg.StandardOutput.BaseStream;
                ffmpeg_Output.CopyTo(memoryString);
                data = memoryString.ToArray();

                Task.WaitAll(inputTask);
            }

            ffmpeg.WaitForExit();
            ffmpeg.Close();

            return data;
        }


        private static IEnumerable<MemoryStream> GetThumbnails(byte[] allImages)
        {
            var bof = allImages.Take(8).ToArray();
            var prevOffset = -1;
            foreach (var offset in GetBytePatternPositions(allImages, bof))
            {
                if (prevOffset > -1)
                    yield return GetImageAt(allImages, prevOffset, offset);
                prevOffset = offset;
            }
            if (prevOffset > -1)
                yield return GetImageAt(allImages, prevOffset, allImages.Length);
        }


        private static MemoryStream GetImageAt(byte[] data, int start, int end)
        {
            using (var ms = new MemoryStream(end - start))
            {
                ms.Write(data, start, end - start);
                return ms;
            }
        }


        private static IEnumerable<int> GetBytePatternPositions(byte[] data, byte[] pattern)
        {
            var dataLen = data.Length;
            var patternLen = pattern.Length - 1;
            int scanData = 0;
            int scanPattern = 0;
            while (scanData < dataLen)
            {
                if (pattern[0] == data[scanData])
                {
                    scanPattern = 1;
                    scanData++;
                    while (pattern[scanPattern] == data[scanData])
                    {
                        if (scanPattern == patternLen)
                        {
                            yield return scanData - patternLen;
                            break;
                        }
                        scanPattern++;
                        scanData++;
                    }
                }
                scanData++;
            }
        }
    }
}
