using System.IO;
using System.IO.Compression;

namespace Roguelike.Utils
{
    // Helper methods for loading xp files
    internal static class RexLoader
    {
        public static char[,] Load (string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            using (GZipStream deflated = new GZipStream(stream, CompressionMode.Decompress))
            using (BinaryReader br = new BinaryReader(deflated))
            {
                int version = br.ReadInt32();
                int layerCount = br.ReadInt32();
                char[,] chars = null;

                for (int i = 0; i < layerCount; i++)
                {
                    int width = br.ReadInt32();
                    int height = br.ReadInt32();
                    if (chars == null)
                        chars = new char[width, height];

                    // TODO: super basic, only gets topmost character, color is ignored
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            chars[x, y] = (char)br.ReadInt32();
                            // foreground rgb
                            br.ReadByte();
                            br.ReadByte();
                            br.ReadByte();
                            // background rgb
                            br.ReadByte();
                            br.ReadByte();
                            br.ReadByte();
                        }
                    }
                }

                return chars;
            }
        }
    }
}
