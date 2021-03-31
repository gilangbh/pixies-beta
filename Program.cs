using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace pixies_nft_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating Pixies!");
            string directory = @"C:\side\pixies-nft\assets\";
            string blankfilePath = @"C:\side\pixies-nft\assets\blank.png";
            string rawfilename = "";
            List<string> hashes = new List<string>();
            int index = 0;

            for (int counter = 0; counter < 16000; counter++)
            {
                index++;
                int finalTier = 3;
                int tier2Count = 0;
                int tier1Count = 0;

                Random rnd = new Random();
                List<string> possibleAsset = new List<string>();

                rawfilename = "";

                int tier = 3;
                int seed = rnd.Next(0, 100);
                if (seed < 10)
                {
                    possibleAsset = Directory.GetFiles($@"{directory}0", "*-t1.png", SearchOption.TopDirectoryOnly).ToList();
                    if (possibleAsset.Count > 0)
                    {
                        tier = 1;
                        tier1Count++;
                    }
                }
                if (tier != 1)
                {
                    seed = rnd.Next(0, 100);
                    if (seed < 20)
                    {
                        possibleAsset = Directory.GetFiles($@"{directory}0", "*-t2.png", SearchOption.TopDirectoryOnly).ToList();
                        if (possibleAsset.Count > 0)
                        {
                            tier = 2;
                            tier2Count++;
                        }
                    }
                }
                if (tier == 3)
                {
                    possibleAsset = Directory.GetFiles($@"{directory}0", "*-t3.png", SearchOption.TopDirectoryOnly).ToList();
                }

                string bgpath = possibleAsset.PickRandom();
                rawfilename += System.IO.Path.GetFileNameWithoutExtension(bgpath);

                bool isLayer9Happen = false;
                bool isLayer11Happen = false;
                bool isLayer12Happen = false;

                using (Image<Rgba32> grid = (Image<Rgba32>)Image.Load(bgpath))
                {
                    Image<Rgba32> blankgrid = (Image<Rgba32>)Image.Load(blankfilePath);

                    for (int i = 1; i < 15; i++)
                    {
                        if (i == 12 && isLayer11Happen)
                            continue;
                        if (i == 14 && (isLayer11Happen || isLayer12Happen))
                            continue;
                        if ((i == 11 || i == 12) && isLayer9Happen)
                            continue;
                        if (i == 13)
                        {
                            rnd = new Random();
                            bool isBoy = rnd.Next() % 2 == 0;
                            if (isBoy)
                                continue;
                        }
                        possibleAsset = new List<string>();
                        tier = 3;
                        seed = rnd.Next(0, 100);
                        if (seed < 10)
                        {
                            possibleAsset = Directory.GetFiles($@"{directory}{i}", "*-t1.png", SearchOption.TopDirectoryOnly).ToList();
                            if (possibleAsset.Count > 0)
                            {
                                tier = 1;
                                tier1Count++;
                            }
                        }
                        if (tier != 1)
                        {
                            seed = rnd.Next(0, 100);
                            if (seed < 20)
                            {
                                possibleAsset = Directory.GetFiles($@"{directory}{i}", "*-t2.png", SearchOption.TopDirectoryOnly).ToList();
                                if (possibleAsset.Count > 0)
                                {
                                    tier = 2;
                                    tier2Count++;
                                }
                            }
                        }
                        if (tier == 3)
                        {
                            possibleAsset = Directory.GetFiles($@"{directory}{i}", "*-t3.png", SearchOption.TopDirectoryOnly).ToList();
                            if(isLayer9Happen && i == 10)
                            {
                                possibleAsset.RemoveAll(x => x.Contains("6-t1"));
                            }
                        }

                        if (i == 9 && tier == 1)
                        {
                            if (tier == 1)
                                isLayer9Happen = true;
                            else
                                isLayer9Happen = false;
                        }
                        if (i == 11 && tier == 1)
                        {
                            if (tier == 1)
                            {
                                isLayer11Happen = true;
                                isLayer12Happen = false;
                            }
                            else
                            {
                                isLayer11Happen = false;
                                isLayer12Happen = true;
                            }
                        }
                        if (i == 12 && tier == 1)
                        {
                            if (tier == 1)
                                isLayer12Happen = true;
                            else
                                isLayer12Happen = false;
                        }

                        string layerPath = possibleAsset.PickRandom();

                        if (!System.IO.Path.GetFileNameWithoutExtension(layerPath).Equals("0-t3"))
                        {
                            rawfilename += System.IO.Path.GetFileNameWithoutExtension(layerPath);

                            Image<Rgba32> layer = (Image<Rgba32>)Image.Load(layerPath);
                            grid.Mutate(ctx => ctx
                                .DrawImage((layer), new Point(0, 0), 1f)
                            );
                            blankgrid.Mutate(ctx => ctx
                                .DrawImage((layer), new Point(0, 0), 1f)
                            );
                            layer.Dispose();
                        }
                    }

                    finalTier = 3;
                    if (tier2Count >= 3 || tier1Count >= 2)
                        finalTier = 2;
                    if (tier1Count >= 3)
                        finalTier = 1;

                    System.Console.WriteLine($"Generating Pixie: Tier 1: {tier1Count} | Tier 2: {tier2Count} | Final Tier: {finalTier}");

                    List<string> frames = new List<string>();

                    frames = Directory.GetFiles($@"{directory}15", $"*-t{finalTier}.png", SearchOption.TopDirectoryOnly).ToList();
                    string framePath = frames.PickRandom();
                    Image<Rgba32> frameLayer = (Image<Rgba32>)Image.Load(framePath);
                    grid.Mutate(ctx => ctx
                        .DrawImage((frameLayer), new Point(0, 0), 1f)
                    );


                    string resultingFileName = GetImagehash(grid);

                    System.Console.WriteLine($"Generating file: {index}-0x" + resultingFileName + ".png | " + (index));
                    using (StreamWriter w = File.AppendText(@"C:\side\pixies-nft\result\log.txt"))
                    {
                        Log($"Generating Pixie: Tier 1: {tier1Count} | Tier 2: {tier2Count} | Final Tier: {finalTier}\nGenerating Pixie: 0x" + resultingFileName + ".png | " + (index), w);
                    }

                    if (!hashes.Exists(x => x.Equals(resultingFileName)))
                    {
                        grid.Save($@"C:\side\pixies-nft\result\tier-{finalTier}\{index}-0x{resultingFileName.ToLower()}.png");
                        grid.Dispose();

                        // blankgrid.Save($@"C:\side\pixies-nft\result\tier-{finalTier}\{index}-0x{resultingFileName.ToLower()}_nobg.png");
                        // blankgrid.Dispose();
                        hashes.Add(resultingFileName);
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText(@"C:\side\pixies-nft\result\log.txt"))
                        {
                            Log("DUPLICATE " + index, w);
                        }
                        index -= 1;
                    }

                }
            }
        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}:");
            w.WriteLine($"{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        public static string GetImagehash(Image<Rgba32> grid)
        {
            using (var ms = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                grid.SaveAsPng(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var sha1 = SHA1.Create();
                sha1.ComputeHash(ms.ToArray());
                foreach (byte b in sha1.ComputeHash(ms.ToArray()))
                    sb.Append(b.ToString("X2"));

                return sb.ToString();
            }
        }
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        private static void GetPixie()
        {
            throw new NotImplementedException();
        }

        private static string TierSelector()
        {
            Random rnd = new Random();
            string result = "t3";

            int seed = rnd.Next(0, 100);
            if (seed < 5)
                result = "t1";
            else if (seed < 20)
                result = "t2";

            return result;
        }
    }
}