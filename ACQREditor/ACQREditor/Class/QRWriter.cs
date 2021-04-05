using ACQREditor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACQREditor.Class
{
    public class QRWriter
    {
        public QRWriter()
        {

        }

        public byte[] Write(DesignInfo design)
        {
            var result = new List<byte>();

            result.AddRange(GetBytes(design.Title, 0x00, 0x29));
            result.AddRange(PadZeroes(2));
            result.AddRange(GetBytes(design.Author, 0x2C, 0x3D));
            result.AddRange(PadZeroes(4));
            result.AddRange(GetBytes(design.Town, 0x42, 0x53));
            result.AddRange(PadZeroes(4));
            // COLOR PALLETTE (TODO)
            result.AddRange(design.RawColorPalette);
            result.AddRange(PadZeroes(1));
            result.Add(0x0A); // FIXED TO 0x0A
            result.Add(0x09); // NORMAL DESIGN
            result.AddRange(PadZeroes(2));
            // DATA (TODO)
            result.AddRange(design.RawDesignData);

            return result.ToArray();
        }

        private byte[] PadZeroes(int length)
        {
            var result = new byte[length];

            Array.Clear(result, 0, result.Length);

            return result;
        }

        private byte[] GetBytes(string value, int start, int end)
        {
            var result = new byte[end - start + 1];
            var bytes = Encoding.UTF8.GetBytes(value);

            for (var i = 0; i < bytes.Length; i++)
                result[i] = bytes[i];

            return result;
        }
    }
}
