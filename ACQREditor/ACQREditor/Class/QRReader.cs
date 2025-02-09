﻿using ACQREditor.Models;
using ACQREditor.Resources;
using SkiaSharp;
using System;
using System.Linq;
using System.Text;

namespace ACQREditor.Class
{
    public class QRReader
    {
        private readonly byte[] pallet_r =
        {
          255, 255, 239, 255, 255, 189, 206, 156, 82,
          255, 255, 222, 255, 255, 206, 189, 189, 140,
          222, 255, 222, 255, 255, 189, 222, 189, 99,
          255, 255, 255, 255, 255, 222, 189, 156, 140,
          255, 239, 206, 189, 206, 156, 140, 82, 49,
          255, 255, 222, 255, 255, 140, 189, 140, 82,
          222, 206, 115, 173, 156, 115, 82, 49, 33,
          255, 255, 222, 255, 255, 206, 156, 140, 82,
          222, 189, 99, 156, 99, 82, 66, 33, 33,
          189, 140, 49, 49, 0, 49, 0, 16, 0,
          156, 99, 33, 66, 0, 82, 33, 16, 0,
          222, 206, 140, 173, 140, 173, 99, 82, 49,
          189, 115, 49, 99, 16, 66, 33, 0, 0,
          173, 82, 0, 82, 0, 66, 0, 0, 0,
          206, 173, 49, 82, 0, 115, 0, 0, 0,
          173, 115, 99, 0, 33, 82, 0, 0, 33,
          255, 239, 222, 206, 189,
          173, 156, 140, 115, 99,
          82, 66, 49, 33, 0
        };

        private readonly byte[] pallet_g =
        {
          239, 154, 85, 101, 0, 69, 0, 0, 32,
          186, 117, 48, 85, 0, 101, 69, 0, 32,
          207, 207, 101, 170, 101, 138, 69, 69, 48,
          239, 223, 207, 186, 170, 138, 101, 85, 69,
          207, 138, 101, 138, 0, 101, 0, 0, 0,
          186, 154, 32, 85, 0, 85, 0, 0, 0,
          186, 170, 69, 117, 48, 48, 32, 16, 16,
          255, 255, 223, 255, 223, 170, 154, 117, 85,
          186, 154, 48, 85, 0, 69, 0, 0, 16,
          186, 154, 48, 85, 0, 48, 0, 16, 0,
          239, 207, 101, 170, 138, 117, 85, 48, 32,
          255, 255, 170, 223, 255, 186, 186, 154, 101,
          223, 207, 85, 154, 117, 117, 69, 32, 16,
          255, 255, 138, 186, 207, 154, 101, 69, 32,
          255, 239, 207, 239, 255, 170, 170, 138, 69,
          255, 255, 223, 255, 223, 186, 186, 138, 69,
          255, 239, 223, 207, 186,
          170, 154, 138, 117, 101,
          85, 69, 48, 32, 0
        };

        private readonly byte[] pallet_b =
        {
            255, 173, 156, 173, 99, 115, 82, 49, 49,
            206, 115, 16, 66, 0, 99, 66, 0, 33,
            189, 99, 33, 33, 0, 82, 0, 0, 16,
            222, 206, 173, 140, 140, 99, 66, 49, 33,
            255, 255, 222, 206, 255, 156, 173, 115, 66,
            255, 255, 189, 239, 206, 115, 156, 99, 66,
            156, 115, 49, 66, 0, 33, 0, 0, 0,
            206, 115, 33, 0, 0, 0, 0, 0, 0,
            255, 239, 206, 255, 255, 140, 156, 99, 49,
            255, 255, 173, 239, 255, 140, 173, 99, 33,
            189, 115, 16, 49, 49, 82, 0, 33, 16,
            189, 140, 82, 140, 0, 156, 0, 0, 0,
            255, 255, 156, 255, 255, 173, 115, 115, 66,
            255, 255, 189, 206, 255, 173, 140, 82, 49,
            239, 222, 173, 189, 206, 173, 156, 115, 49,
            173, 115, 66, 0, 33, 82, 0, 0, 33,
            255, 239, 222, 206, 189,
            173, 156, 140, 115, 99,
            82, 66, 49, 33, 0
        };

        public QRReader()
        {

        }

        public QRResponse Read(byte[] fullQR)
        {
            // byte encoding flag and qr code size for normal patterns
            if (fullQR[0] != 0x40 || // byte encoding
                fullQR[1] != 0x26 || // QR size (12-bit), first 8-bits
                fullQR[2] >> 4 != 0x0C) // QR size (12-bit), last 4 bits
            {
                return new QRResponse
                {
                    Success = false,
                    Message = Labels.OnlyNormalPatternsSupported
                };
            }

            // HACK!!!! to skip 2.5 bytes ( 2 bytes and 4 bits )
            string byteString = ByteArrayToHexString(fullQR);
            byteString = byteString.Substring(5) + "0";
            byte[] bytes = HexStringToByteArray(byteString);

            // extract necessary information
            var info = new DesignInfo
            {
                Title = GetString(bytes, 0, 0x29),
                Author = GetString(bytes, 0x2C, 0x3D),
                Town = GetString(bytes, 0x42, 0x53),
                RawColorPalette = GetBytes(bytes, 0x58, 0x66),
                RawDesignData = GetBytes(bytes, 0x6C, bytes.Length).Take(512).ToArray()
            };

            if (bytes[0x69] != 0x09)
            {
                return new QRResponse
                {
                    Success = false,
                    Message = Labels.OnlyNormalPatternsSupported
                };
            }

            info.Bitmap = CreateBitmap(info.RawDesignData, info.RawColorPalette);

            return new QRResponse
            {
                Success = true,
                Design = info
            };
        }

        private byte[] HexStringToByteArray(string values)
        {
            return Enumerable.Range(0, values.Length / 2)
                .Select(x => Convert.ToByte(values.Substring(x * 2, 2), 16))
                .ToArray();
        }

        private string ByteArrayToHexString(byte[] values)
        {
            var builder = new StringBuilder(values.Length * 2);

            foreach (var b in values)
                builder.AppendFormat("{0:x2}", b);

            return builder.ToString();
        }

        private string GetString(byte[] original, int from, int to)
        {
            return Encoding.UTF8.GetString(GetBytes(original, from, to))?.Replace("\0", string.Empty);
        }

        private byte[] GetBytes(byte[] original, int from, int to)
        {
            var realTo = to - from + 1;

            return original.Skip(from).Take(realTo).ToArray();
        }

        private SKBitmap CreateBitmap(byte[] design, byte[] colorPalette)
        {
            //Data pata consists from above color palette, 0 - 15 indexed number, 4bits per pixel.
            const int size = 32;
            var bitmap = new SKBitmap(size, size, true);

            var pos = new SKPointI(0, 0);

            foreach (var designByte in design)
            {
                // 2 colors per byte, 4 bits per color
                var color1 = CalculateColor(designByte >> 4, colorPalette); // first 4-bits
                var color2 = CalculateColor(designByte & 0x0F, colorPalette); // last 4-bits

                bitmap.SetPixel(pos.X, pos.Y, color2);
                pos = IncrementPosition(pos, size);

                bitmap.SetPixel(pos.X, pos.Y, color1);
                pos = IncrementPosition(pos, size);
            }

            return bitmap;
        }

        private SKPointI IncrementPosition(SKPointI pos, int max)
        {
            pos.X++;

            if (pos.X >= max)
            {
                pos.X = 0;
                pos.Y++;
            }

            return pos;
        }

        private SKColor CalculateColor(int designByte, byte[] colorPalette)
        {
            if (designByte == 15) // transparent
                return new SKColor(0, 0, 0, 0);

            var colorReference = colorPalette[designByte];

            int index;
            var matrix = colorReference >> 4; // first 4-bits
            var offset = colorReference & 0x0F; // last 4-bits

            if (offset == 0x000F)
                index = 144 + matrix; // grayscale, starts at 145
            else
                index = (matrix * 9) + offset; // from 9-color matrix

            var r = pallet_r[index];
            var g = pallet_g[index];
            var b = pallet_b[index];

            return new SKColor(r, g, b);
        }

    }
}
