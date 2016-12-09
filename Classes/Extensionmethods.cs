#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace IV_Play
{
    public static class ExtensionMethods
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static T DeepCopy<T>(this T source)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, source);
                ms.Position = 0;
                return (T)binaryFormatter.Deserialize(ms);
            }
        }

        public static bool In(this string instring, params string[] strings)
        {
            return (strings.Any(s => s.Equals(instring)));
        }

        public static void DrawRectangleF(this Graphics graphics, Pen pen, RectangleF rectangleF)
        {
            graphics.DrawRectangle(pen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
        }

        public static System.Windows.Media.Brush ToBrush(this System.Drawing.Color color)
        {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}