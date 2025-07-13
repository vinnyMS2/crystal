using System.IO;

namespace PngToMap
{
    public class CellInfo
    {
        public short BackIndex;
        public int BackImage;
        public short MiddleIndex;
        public short MiddleImage;
        public short FrontIndex;
        public short FrontImage;
        public byte DoorIndex;
        public byte DoorOffset;
        public byte FrontAnimationFrame;
        public byte FrontAnimationTick;
        public byte MiddleAnimationFrame;
        public byte MiddleAnimationTick;
        public short TileAnimationImage;
        public short TileAnimationOffset;
        public byte TileAnimationFrames;
        public byte Light;
        public bool FishingCell;
    }

    public class Map
    {
        private readonly int _width;
        private readonly int _height;
        private readonly CellInfo[,] _cells;

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            _cells = new CellInfo[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = new CellInfo();
                }
            }
        }

        public void SetTile(int x, int y, short backIndex, int backImage)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return;
            }
            _cells[x, y].BackIndex = backIndex;
            _cells[x, y].BackImage = backImage;
        }

        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((short)1); // Version
                writer.Write('C');
                writer.Write('#');
                writer.Write((short)_width);
                writer.Write((short)_height);

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        CellInfo cell = _cells[x, y];
                        writer.Write(cell.BackIndex);
                        writer.Write(cell.BackImage);
                        writer.Write(cell.MiddleIndex);
                        writer.Write(cell.MiddleImage);
                        writer.Write(cell.FrontIndex);
                        writer.Write(cell.FrontImage);
                        writer.Write(cell.DoorIndex);
                        writer.Write(cell.DoorOffset);
                        writer.Write(cell.FrontAnimationFrame);
                        writer.Write(cell.FrontAnimationTick);
                        writer.Write(cell.MiddleAnimationFrame);
                        writer.Write(cell.MiddleAnimationTick);
                        writer.Write(cell.TileAnimationImage);
                        writer.Write(cell.TileAnimationOffset);
                        writer.Write(cell.TileAnimationFrames);
                        writer.Write(cell.Light);
                    }
                }
            }
        }
    }
}
