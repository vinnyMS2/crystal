using System.Drawing;
using System.IO.Compression;

namespace PngToMap
{
    public sealed class MLibrary
    {
        public const int LibVersion = 3;
        public string FileName;

        public List<MImage> Images = new List<MImage>();
        public List<int> IndexList = new List<int>();
        public int Count;

        public MLibrary(string filename)
        {
            FileName = filename + ".lib";
        }

        public void Save()
        {
            using (FileStream stream = File.Create(FileName))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Count = Images.Count;
                IndexList.Clear();

                int offSet = 8 + Count * 4;
                for (int i = 0; i < Count; i++)
                {
                    IndexList.Add((int)stream.Length + offSet);
                    Images[i].Save(writer);
                }

                writer.Write(LibVersion);
                writer.Write(Count);
                for (int i = 0; i < Count; i++)
                    writer.Write(IndexList[i]);

                for (int i = 0; i < Count; i++)
                {
                    writer.Write(Images[i].FBytes);
                }
            }
        }

        public void AddImage(Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };
            Images.Add(mImage);
        }

        public sealed class MImage
        {
            public short Width, Height, X, Y, ShadowX, ShadowY;
            public byte Shadow;
            public int Length;
            public byte[] FBytes;

            public MImage(Bitmap image)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;

                FBytes = ConvertBitmapToArray(image);
            }

            private byte[] ConvertBitmapToArray(Bitmap input)
            {
                BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[input.Width * input.Height * 4];

                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

                input.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                byte[] compressedBytes;
                compressedBytes = Compress(pixels);

                return compressedBytes;
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(ShadowX);
                writer.Write(ShadowY);
                writer.Write(Shadow);
                writer.Write(FBytes.Length);
            }

            public static byte[] Compress(byte[] raw)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                    {
                        gzip.Write(raw, 0, raw.Length);
                    }
                    return memory.ToArray();
                }
            }
        }
    }
}
