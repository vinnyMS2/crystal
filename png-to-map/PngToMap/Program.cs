using System.Drawing;
using System.Security.Cryptography;

namespace PngToMap
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "input.png";
            string outputLibFile = "output";
            string outputMapFile = "output.map";
            int tileSize = 48;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--input" && i + 1 < args.Length)
                {
                    inputFile = args[i + 1];
                }
                else if (args[i] == "--output" && i + 1 < args.Length)
                {
                    outputLibFile = args[i + 1];
                    outputMapFile = args[i + 1] + ".map";
                }
                else if (args[i] == "--tileSize" && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out tileSize);
                }
            }

            SampleGenerator.Generate(inputFile);

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Input file not found: {inputFile}");
                return;
            }

            using (Bitmap image = new Bitmap(inputFile))
            {
                MLibrary library = new MLibrary(outputLibFile);
                Map map = new Map(image.Width / tileSize, image.Height / tileSize);
                Dictionary<string, int> tileHashes = new Dictionary<string, int>();
                int tileIndex = 0;

                for (int y = 0; y < image.Height; y += tileSize)
                {
                    for (int x = 0; x < image.Width; x += tileSize)
                    {
                        Rectangle tileBounds = new Rectangle(x, y, tileSize, tileSize);
                        using (Bitmap tile = image.Clone(tileBounds, image.PixelFormat))
                        {
                            string hash = GetBitmapHash(tile);
                            if (!tileHashes.ContainsKey(hash))
                            {
                                tileHashes.Add(hash, tileIndex);
                                library.AddImage(tile, 0, 0);
                                tileIndex++;
                            }
                            map.SetTile(x / tileSize, y / tileSize, 0, tileHashes[hash] + 1);
                        }
                    }
                }

                library.Save();
                map.Save(outputMapFile);
                Console.WriteLine($"Successfully created {outputLibFile}.lib and {outputMapFile}");
            }
        }

        private static string GetBitmapHash(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(stream.ToArray());
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
