﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace wav2nub
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] mt5Header = { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0xF0, 0x38, 0xD2, 0x03, 0x20, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x77, 0x61, 0x76, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x38, 0xD2, 0x03, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x38, 0xD2, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x42, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0xC2, 0xE8, 0x03, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x44, 0xAC, 0x00, 0x00, 0x10, 0xB1, 0x02, 0x00, 0x04, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Console.WriteLine("wav2nub");
            Console.WriteLine("Created by nzgamer41");

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: wav2nub.exe <path to snd file>");
            }

            else
            {
                try
                {
                    byte[] file = File.ReadAllBytes(args[0]);

                    Array.Reverse(file);

                    var s = new RawSourceWaveStream(new MemoryStream(file), new WaveFormat(44100, 16, 2));
                    var outpath = "temp.raw";
                    int blockAlign = s.WaveFormat.BlockAlign;
                    using (WaveFileWriter writer = new WaveFileWriter(outpath, s.WaveFormat))
                    {
                        byte[] buffer = new byte[blockAlign];
                        long samples = s.Length / blockAlign;
                        for (long sample = samples - 1; sample >= 0; sample--)
                        {
                            s.Position = sample * blockAlign;
                            s.Read(buffer, 0, blockAlign);
                            writer.WriteData(buffer, 0, blockAlign);
                        }
                    }

                    ReplaceData("temp.raw", 0, mt5Header);
                    if (!File.Exists(Path.ChangeExtension(args[0], ".nub")))
                    {
                        File.Move("temp.raw", Path.ChangeExtension(args[0], ".nub"));
                    }
                    else
                    {
                        File.Delete(Path.ChangeExtension(args[0], ".nub"));
                        File.Move("temp.raw", Path.ChangeExtension(args[0], ".nub"));
                    }
                    Console.WriteLine(args[0] + " converted to " + Path.ChangeExtension(args[0], ".nub"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void ReplaceData(string filename, int position, byte[] data)
        {
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                stream.Position = position;
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
