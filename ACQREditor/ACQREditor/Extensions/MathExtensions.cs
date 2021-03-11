using SkiaSharp;
using System;

namespace ACQREditor.Extensions
{
    public static class MathExtensions
    {

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) 
                return min;

            else if (val.CompareTo(max) > 0) 
                return max;

            else return val;
        }


        public static float Magnitude(this SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }

    }
}
