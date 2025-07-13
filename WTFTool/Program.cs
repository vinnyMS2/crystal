using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WTFTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: WTFTool.exe <input.png> <mapping.json> <output.map>");
                return;
            }

            string pngPath = args[0];
            string mappingPath = args[1];
            string outputPath = args[2];

            // Load the mapping file
            List<ColorMapping> mappings = JsonConvert.DeserializeObject<List<ColorMapping>>(File.ReadAllText(mappingPath));

            // Load the PNG image
            using (Image<Rgba32> image = Image.Load<Rgba32>(pngPath))
            {
                // Create the map data
                Map map = new Map(image.Width, image.Height);

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Rgba32 pixelColor = image[x, y];
                        string hexColor = "#" + pixelColor.R.ToString("X2") + pixelColor.G.ToString("X2") + pixelColor.B.ToString("X2");

                        ColorMapping mapping = mappings.Find(m => m.Color.Equals(hexColor, StringComparison.OrdinalIgnoreCase));

                        if (mapping != null)
                        {
                            map.Cells[x, y] = new CellInfo
                            {
                                BackIndex = mapping.Tile.BackIndex,
                                BackImage = mapping.Tile.BackImage,
                                MiddleIndex = mapping.Tile.MiddleIndex,
                                MiddleImage = mapping.Tile.MiddleImage,
                                FrontIndex = mapping.Tile.FrontIndex,
                                FrontImage = mapping.Tile.FrontImage,
                                DoorIndex = mapping.Tile.DoorIndex,
                                DoorOffset = mapping.Tile.DoorOffset,
                                FrontAnimationFrame = mapping.Tile.FrontAnimationFrame,
                                FrontAnimationTick = mapping.Tile.FrontAnimationTick,
                                MiddleAnimationFrame = mapping.Tile.MiddleAnimationFrame,
                                MiddleAnimationTick = mapping.Tile.MiddleAnimationTick,
                                TileAnimationImage = mapping.Tile.TileAnimationImage,
                                TileAnimationOffset = mapping.Tile.TileAnimationOffset,
                                TileAnimationFrames = mapping.Tile.TileAnimationFrames,
                                Light = mapping.Tile.Light
                            };
                        }
                        else
                        {
                            // Default tile if no mapping is found
                            map.Cells[x, y] = new CellInfo();
                        }
                    }
                }

                // Save the map file
                map.Save(outputPath);
            }

            Console.WriteLine("Map saved successfully!");
        }
    }

    public class ColorMapping
    {
        public string Color { get; set; }
        public CellInfo Tile { get; set; } = new CellInfo();
    }

    public class Map
    {
        public short Width { get; }
        public short Height { get; }
        public CellInfo[,] Cells { get; }

        public Map(int width, int height)
        {
            Width = (short)width;
            Height = (short)height;
            Cells = new CellInfo[Width, Height];
        }

        public void Save(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                short ver = 1;
                char[] tag = { 'C', '#' };
                binaryWriter.Write(ver);
                binaryWriter.Write(tag);

                binaryWriter.Write(Width);
                binaryWriter.Write(Height);

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        var cell = Cells[x, y];
                        binaryWriter.Write(cell.BackIndex);
                        binaryWriter.Write(cell.BackImage);
                        binaryWriter.Write(cell.MiddleIndex);
                        binaryWriter.Write(cell.MiddleImage);
                        binaryWriter.Write(cell.FrontIndex);
                        binaryWriter.Write(cell.FrontImage);
                        binaryWriter.Write(cell.DoorIndex);
                        binaryWriter.Write(cell.DoorOffset);
                        binaryWriter.Write(cell.FrontAnimationFrame);
                        binaryWriter.Write(cell.FrontAnimationTick);
                        binaryWriter.Write(cell.MiddleAnimationFrame);
                        binaryWriter.Write(cell.MiddleAnimationTick);
                        binaryWriter.Write(cell.TileAnimationImage);
                        binaryWriter.Write(cell.TileAnimationOffset);
                        binaryWriter.Write(cell.TileAnimationFrames);
                        binaryWriter.Write(cell.Light);
                    }
                }
            }
        }
    }

    public class CellInfo
    {
        public short BackIndex { get; set; }
        public int BackImage { get; set; }
        public short MiddleIndex { get; set; }
        public short MiddleImage { get; set; }
        public short FrontIndex { get; set; }
        public short FrontImage { get; set; }
        public byte DoorIndex { get; set; }
        public byte DoorOffset { get; set; }
        public byte FrontAnimationFrame { get; set; }
        public byte FrontAnimationTick { get; set; }
        public byte MiddleAnimationFrame { get; set; }
        public byte MiddleAnimationTick { get; set; }
        public short TileAnimationImage { get; set; }
        public short TileAnimationOffset { get; set; }
        public byte TileAnimationFrames { get; set; }
        public byte Light { get; set; }

        public CellInfo()
        {
            BackIndex = 0;
            BackImage = 0;
            MiddleIndex = 0;
            MiddleImage = 0;
            FrontIndex = 0;
            FrontImage = 0;
            DoorIndex = 0;
            DoorOffset = 0;
            FrontAnimationFrame = 0;
            FrontAnimationTick = 0;
            MiddleAnimationFrame = 0;
            MiddleAnimationTick = 0;
            TileAnimationImage = 0;
            TileAnimationOffset = 0;
            TileAnimationFrames = 0;
            Light = 0;
        }
    }
}
