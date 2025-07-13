using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using WTFTool;

namespace WTFTool.Tests
{
    [TestClass]
    public class DirectMappingTests
    {
        [TestMethod]
        public void TestDirectMapping()
        {
            // Create a dummy mapping list
            var mappings = new List<ColorMapping>
            {
                new ColorMapping
                {
                    Color = "#FFFFFF",
                    Tile = new CellInfo { BackIndex = 1, BackImage = 1 }
                },
                new ColorMapping
                {
                    Color = "#000000",
                    Tile = new CellInfo { BackIndex = 2, BackImage = 2 }
                }
            };

            // Test the mapping for the black pixel
            string hexColor = "#000000";
            ColorMapping mapping = mappings.Find(m => m.Color.Equals(hexColor, System.StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(mapping);
            Assert.AreEqual(2, mapping.Tile.BackIndex);
            Assert.AreEqual(2, mapping.Tile.BackImage);
        }
    }
}
