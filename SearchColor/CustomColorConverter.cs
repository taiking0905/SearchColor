using System;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace SearchColor
{
    public static class CustomColorConverter
    {
        public static Color HSVToRGB(double h, double s, double v)
        {
            h = h % 360;
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r = 0, g = 0, b = 0;

            if (0 <= h && h < 60) { r = c; g = x; b = 0; }
            else if (60 <= h && h < 120) { r = x; g = c; b = 0; }
            else if (120 <= h && h < 180) { r = 0; g = c; b = x; }
            else if (180 <= h && h < 240) { r = 0; g = x; b = c; }
            else if (240 <= h && h < 300) { r = x; g = 0; b = c; }
            else if (300 <= h && h < 360) { r = c; g = 0; b = x; }

            byte R = (byte)((r + m) * 255);
            byte G = (byte)((g + m) * 255);
            byte B = (byte)((b + m) * 255);

            return Color.FromRgb(R, G, B);
        }

        public static void RGBToHSV(byte r, byte g, byte b, out double h, out double s, out double v)
        {
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            double delta = max - min;

            if (delta == 0) h = 0;
            else if (max == rNorm) h = 60 * (((gNorm - bNorm) / delta) % 6);
            else if (max == gNorm) h = 60 * (((bNorm - rNorm) / delta) + 2);
            else h = 60 * (((rNorm - gNorm) / delta) + 4);

            if (h < 0) h += 360;

            s = (max == 0) ? 0 : (delta / max);
            v = max;
        }
    }
}
