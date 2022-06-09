using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TModel.MainFrame.Widgets
{
    public class CScrollViewer : ScrollViewer
    {
        bool IsDragging;
        double StartMouse;
        double StartOffset;

        public CScrollViewer() : base()
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Center;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            MouseRightButtonDown += (sender, args) =>
            {
                IsDragging = true;
                StartMouse = (Mouse.GetPosition(this).Y - ActualHeight) * -1;
                StartOffset = VerticalOffset;
            };

            App.Window.MouseRightButtonUp += (sender, args) =>
            {
                IsDragging = false;
            };

            App.Window.MouseMove += (sender, args) =>
            {
                double Position = Mouse.GetPosition(this).Y;
                
                if (IsDragging)
                {
                    ScrollToVerticalOffset((((Position - ActualHeight) * -1) - StartMouse) + StartOffset);
                }
            };

            App.Window.MouseLeave += (sender, args) => IsDragging = false;
        }
    }
}
