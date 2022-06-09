using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TModel
{
    public static class GridUtils
    {
        public static void AddRow(this Grid grid, UIElement element)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(element, grid.RowDefinitions.Count);
            grid.Children.Add(element);
        }

        public static void AddRow2Columns(this Grid grid, UIElement column1, UIElement column2)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(column1, grid.RowDefinitions.Count);
            Grid.SetRow(column2, grid.RowDefinitions.Count);
            Grid.SetColumn(column2, 1);
            grid.Children.Add(column1);
            grid.Children.Add(column2);
        }
    }
}
