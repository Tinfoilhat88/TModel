using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TModel
{
    public static class ColorConverters
    {
        static BrushConverter brushConverter = new BrushConverter();

        public static Color HexColor(string hex) => (Color)ColorConverter.ConvertFromString(hex);

        public static Brush HexBrush(string hex)
        {
            // Makes sure hex starts with '#'
            hex = hex.StartsWith("#") ? hex : '#' + hex;
            return (Brush)(brushConverter.ConvertFromString(hex));
        }

        public static Brush Solid(this Color color) => new SolidColorBrush(color);
    }
}
