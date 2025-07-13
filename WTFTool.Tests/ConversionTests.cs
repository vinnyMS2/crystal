using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WTFTool;

namespace WTFTool.Tests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void TestPngToMapConversion()
        {
            // Create a dummy PNG file
            string pngPath = "test.png";
            using (Image<Rgba32> image = new Image<Rgba32>(2, 2))
            {
                image[0, 0] = new Rgba32(255, 255, 255);
                image[0, 1] = new Rgba32(0, 0, 0);
                image[1, 0] = new Rgba32(255, 0, 0);
                image[1, 1] = new Rgba32(0, 0, 255);
                image.Save(pngPath);
            }

            // Create a dummy mapping file
            string mappingPath = "mapping.json";
            string mappingJson = @"
[
  {
    ""color"": ""#FFFFFF"",
    ""tile"": { ""BackIndex"": 1, ""BackImage"": 1 }
  },
  {
    ""color"": ""#000000"",
    ""tile"": { ""BackIndex"": 2, ""BackImage"": 2 }
  }
]";
            File.WriteAllText(mappingPath, mappingJson);

            // Run the conversion
            string outputPath = "test.map";
            Program.Main(new string[] { pngPath, mappingPath, outputPath });

            // Verify the output
            Assert.IsTrue(File.Exists(outputPath));

            using (var fileStream = new FileStream(outputPath, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                short ver = binaryReader.ReadInt16();
                char[] tag = binaryReader.ReadChars(2);
                short width = binaryReader.ReadInt16();
                short height = binaryReader.ReadInt16();

                Assert.AreEqual(1, ver);
                Assert.AreEqual('C', tag[0]);
                Assert.AreEqual('#', tag[1]);
                Assert.AreEqual(2, width);
                Assert.AreEqual(2, height);

                // Check the cell data
                // Pixel (0,0) - White
                Assert.AreEqual(1, binaryReader.ReadInt16()); // BackIndex
                Assert.AreEqual(1, binaryReader.ReadInt32()); // BackImage
                // ... (check the rest of the fields)
                 for (int i = 0; i < 14; i++) binaryReader.ReadByte();


                // Pixel (0,1) - Black
                Assert.AreEqual(2, binaryReader.ReadInt16()); // BackIndex
                Assert.AreEqual(2, binaryReader.ReadInt32()); // BackImage
                 for (int i = 0; i < 14; i++) binaryReader.ReadByte();

                // Pixel (1,0) - Red (unmapped)
                Assert.AreEqual(0, binaryReader.ReadInt16()); // BackIndex
                Assert.AreEqual(0, binaryReader.ReadInt32()); // BackImage
                 for (int i = 0; i < 14; i++) binaryReader.ReadByte();

                // Pixel (1,1) - Blue (unmapped)
                Assert.AreEqual(0, binaryReader.ReadInt16()); // BackIndex
                Assert.AreEqual(0, binaryReader.ReadInt32()); // BackImage
                 for (int i = 0; i < 14; i++) binaryReader.ReadByte();
            }

            // Clean up
            File.Delete(pngPath);
            File.Delete(mappingPath);
            File.Delete(outputPath);
        }
    }
}
