using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static TModel.ColorConverters;

public static class Theme
{
    public static FontFamily CoreFont = new FontFamily("Segoe UI");

    public static Brush BackDark = HexBrush("0f1721");

    // Background
    public static Brush BackNormal = HexBrush("#1a232d");
    public static Brush BackHover = HexBrush("#1e64a5");
    public static Brush BackSelected = HexBrush("#448af6");

    // Border
    public static Brush BorderNormal = HexBrush("#2a3d53");
    public static Brush BorderHover = HexBrush("#6383a8");
    public static Brush BorderSelected = HexBrush("#f9ff6e");
}

namespace TModel
{


    public class CTextBox : TextBox
    {
        public CTextBox()
        {
            FontFamily = Theme.CoreFont;
            Foreground = Brushes.White;
            Background = HexBrush("#0f1243");
            Padding = new Thickness(6);
            FontSize = 15;
            BorderThickness = new Thickness(2);
            BorderBrush = HexBrush("#3144a0");
            VerticalContentAlignment = VerticalAlignment.Center;
        }
    }

    public class CTooltip : ToolTip
    {
        static Brush background = HexBrush("#070032");
        static Brush Border = HexBrush("#412db3");
        public CTooltip(string text)
        {
            Content = new CTextBlock(text);
            Background = background;
            Padding = new Thickness(5);
            BorderThickness = new Thickness(1   );
            BorderBrush = Border;
            Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        }
    }

    public class CTextBlock : TextBlock
    {
        public CTextBlock(string text, double size = 15, Thickness? margin = null)
        {
            Text = text;
            Foreground = Brushes.White;
            Margin = margin ?? new Thickness(0);
            FontSize = size;
        }

        public CTextBlock() : this("")
        {

        }
    }

    public class ReadonlyText : TextBox
    {
        public ReadonlyText(double size = 15)
        {
            FontSize = size;
            IsReadOnly = true;
            Foreground = Brushes.White;
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
        }

        public ReadonlyText(string text, double size = 15) : this(size)
        {
            Text = text;
        }
    }
}
