// THIS CODE HAS NOT BEEN COMPILED OR TESTED
// The environment does not have a C# compiler available.

using System;
using System.IO;
using System.Drawing;
using Map_Editor;

namespace MapConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MapConverter <input.png> <output.map> [color_map.json]");
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];
            string colorMapFile = null;

            if (args.Length > 2)
            {
                colorMapFile = args[2];
            }

            Console.WriteLine($"Input PNG: {inputFile}");
            Console.WriteLine($"Output MAP: {outputFile}");
            if (colorMapFile != null)
            {
                Console.WriteLine($"Color Map: {colorMapFile}");
            }

            try
            {
                CellInfo[,] mapCells = ConvertPngToCellInfo(inputFile, colorMapFile);
                SaveMap(mapCells, mapCells.GetLength(0), mapCells.GetLength(1), outputFile);
                Console.WriteLine("Conversion successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static CellInfo[,] ConvertPngToCellInfo(string inputFile, string colorMapFile)
        {
            // TODO: Load the color map from the colorMapFile (e.g., a JSON file).
            // The color map should be a dictionary where the key is the color
            // and the value is a CellInfo object.
            var colorMap = new System.Collections.Generic.Dictionary<Color, CellInfo>
            {
                { Color.FromArgb(255, 0, 0, 0), new CellInfo { BackIndex = 0, BackImage = 1 } }, // Black
                { Color.FromArgb(255, 255, 255, 255), new CellInfo { BackIndex = 0, BackImage = 2 } } // White
            };

            using (var bitmap = new Bitmap(inputFile))
            {
                var mapCells = new CellInfo[bitmap.Width, bitmap.Height];
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        if (colorMap.TryGetValue(pixelColor, out CellInfo cellInfo))
                        {
                            mapCells[x, y] = cellInfo;
                        }
                        else
                        {
                            // Default to a black tile if the color is not in the map
                            mapCells[x, y] = new CellInfo { BackIndex = 0, BackImage = 1 };
                        }
                    }
                }
                return mapCells;
            }
        }

        static void SaveMap(CellInfo[,] mapCells, int width, int height, string outputFile)
        {
            // This method saves the map data to a file in the custom format.
            // The format is as follows:
            // 1. A 2-byte version number (short).
            // 2. A 2-byte character tag ('C', '#').
            // 3. A 2-byte integer for the map width (short).
            // 4. A 2-byte integer for the map height (short).
            // 5. The cell data, which is a sequence of CellInfo objects.
            using (var fileStream = new FileStream(outputFile, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                short ver = 1;
                char[] tag = { 'C', '#' };
                binaryWriter.Write(ver);
                binaryWriter.Write(tag);

                binaryWriter.Write(Convert.ToInt16(width));
                binaryWriter.Write(Convert.ToInt16(height));
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        binaryWriter.Write(mapCells[x, y].BackIndex);
                        binaryWriter.Write(mapCells[x, y].BackImage);
                        binaryWriter.Write(mapCells[x, y].MiddleIndex);
                        binaryWriter.Write(mapCells[x, y].MiddleImage);
                        binaryWriter.Write(mapCells[x, y].FrontIndex);
                        binaryWriter.Write(mapCells[x, y].FrontImage);
                        binaryWriter.Write(mapCells[x, y].DoorIndex);
                        binaryWriter.Write(mapCells[x, y].DoorOffset);
                        binaryWriter.Write(mapCells[x, y].FrontAnimationFrame);
                        binaryWriter.Write(mapCells[x, y].FrontAnimationTick);
                        binaryWriter.Write(mapCells[x, y].MiddleAnimationFrame);
                        binaryWriter.Write(mapCells[x, y].MiddleAnimationTick);
                        binaryWriter.Write(mapCells[x, y].TileAnimationImage);
                        binaryWriter.Write(mapCells[x, y].TileAnimationOffset);
                        binaryWriter.Write(mapCells[x, y].TileAnimationFrames);
                        binaryWriter.Write(mapCells[x, y].Light);
                    }
                }
            }
        }
    }
}
