using SkiaSharp;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ACQREditor.Models
{
    public class DesignInfo
    {

        public string Author { get; set; }
        public string Town { get; set; }
        public string Title { get; set; }


        public byte[] RawColorPalette { get; set; }
        public byte[] RawDesignData { get; set; }
        public List<Color> RawRGBData { get; set; }
        public SKBitmap Bitmap { get; set; }

    }
}
