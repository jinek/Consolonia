using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Consolonia.Core.Text.Fonts
{
    public static class AsciiArtUtilities
    {
        public static bool IsCompressedFile(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[4];
            if (stream.Read(buffer, 0, 4) < 4)
                return false;

            // Check for ZIP/PKZIP signature (PK\x03\x04 or 0x04034b50)
            bool isZip = buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04;
            return isZip;
        }

        public static string[] ReadFontStream(Stream stream)
        {
            if (IsCompressedFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);

                using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

                ZipArchiveEntry entry = archive.GetEntry("-");
                if (entry == null)
                    entry = archive.Entries.FirstOrDefault();

                if (entry == null)
                    throw new InvalidDataException("Compressed font file contains no entries");

                using Stream entryStream = entry.Open();
                using var reader = new StreamReader(entryStream);

                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null) lines.Add(line);

                return lines.ToArray();
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null) lines.Add(line);
                return lines.ToArray();
            }
        }
    }
}