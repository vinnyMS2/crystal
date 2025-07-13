using System.Drawing;
using System.Drawing.Imaging;

namespace PngToMap
{
    public class SampleGenerator
    {
        public static void Generate(string fileName)
        {
            const int tileSize = 48;
            const int width = 4;
            const int height = 4;

            using (Bitmap image = new Bitmap(width * tileSize, height * tileSize))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.FillRectangle(Brushes.Red, new Rectangle(0, 0, tileSize, tileSize));
                    g.FillRectangle(Brushes.Green, new Rectangle(tileSize, 0, tileSize, tileSize));
                    g.FillRectangle(Brushes.Blue, new Rectangle(2 * tileSize, 0, tileSize, tileSize));
                    g.FillRectangle(Brushes.Yellow, new Rectangle(3 * tileSize, 0, tileSize, tileSize));

                    g.FillRectangle(Brushes.Yellow, new Rectangle(0, tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Blue, new Rectangle(tileSize, tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Green, new Rectangle(2 * tileSize, tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Red, new Rectangle(3 * tileSize, tileSize, tileSize, tileSize));

                    g.FillRectangle(Brushes.Red, new Rectangle(0, 2 * tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Green, new Rectangle(tileSize, 2 * tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Blue, new Rectangle(2 * tileSize, 2 * tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Yellow, new Rectangle(3 * tileSize, 2 * tileSize, tileSize, tileSize));

                    g.FillRectangle(Brushes.Yellow, new Rectangle(0, 3*tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Blue, new Rectangle(tileSize, 3*tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Green, new Rectangle(2*tileSize, 3*tileSize, tileSize, tileSize));
                    g.FillRectangle(Brushes.Red, new Rectangle(3*tileSize, 3*tileSize, tileSize, tileSize));
                }
                image.Save(fileName, ImageFormat.Png);
            }
        }
    }
}
