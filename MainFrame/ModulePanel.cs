using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static TModel.ColorConverters;

namespace TModel
{
    // Contains an array of ModuleContainers that are seperated by GridSplitters
    public sealed class ModulePanel : Grid
    {
        private Orientation Direction;

        private Orientation OppositeDirection => Direction == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;

        private int ModuleCount => Direction == Orientation.Horizontal ? ColumnDefinitions.Count : RowDefinitions.Count;

        private List<ModuleContainer> Containers { get; } = new List<ModuleContainer>();

        public ModulePanel(Orientation direction = Orientation.Horizontal)
        {
            Direction = direction;
        }

        // Replaces the ContainerToReplace with a new ModulePanel of the OppositeDirection
        // containing the module that was there before and the NewModule.
        public void MakeSeperator(ModuleContainer ContainerToReplace, ModuleContainer NewModule, GridLength? length = null)
        {
            if (Containers.IndexOf(ContainerToReplace) == -1) throw new ArgumentException("Module does not exist in panel", nameof(ContainerToReplace));
            int Index = Children.IndexOf(ContainerToReplace);
            Children.Remove(ContainerToReplace);
            ModulePanel NewModulePanel = new ModulePanel(OppositeDirection);
            NewModulePanel.AddModule(ContainerToReplace);
            NewModule.ParentPanel = NewModulePanel;
            NewModulePanel.AddModule(NewModule);
            if (Direction == Orientation.Horizontal)
                SetColumn(NewModulePanel, Index);
            else
                SetRow(NewModulePanel, Index);
            Children.Add(NewModulePanel);
            if (length is GridLength ValidLength)
            {
                if (Direction == Orientation.Horizontal)
                    NewModulePanel.RowDefinitions[NewModulePanel.RowDefinitions.Count - 1].Height = ValidLength;
                else
                    NewModulePanel.ColumnDefinitions[NewModulePanel.ColumnDefinitions.Count - 1].Width = ValidLength;
            }
        }

        public bool TryShowModule<T>()
        {
            foreach (var item in Containers)
                if (item.TryShowModule<T>())
                    return true;
            return false;
        }

        private void AddSplitter()
        {
            if (ModuleCount > 0)
            {
                GridSplitter gridSplitter = new GridSplitter()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = HexBrush("#2d2d2d"),
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext
                };
                if (Direction == Orientation.Horizontal)
                {
                    gridSplitter.Width = 8;
                    gridSplitter.ResizeDirection = GridResizeDirection.Columns;
                }
                else
                {
                    gridSplitter.Height = 8;
                    gridSplitter.ResizeDirection = GridResizeDirection.Rows;
                }
                AddElement(gridSplitter, true);
            }
        }

        public void AddModule(ModuleContainer moduleContainer, double ratio = 1.0)
        {
            AddSplitter();
            Containers.Add(moduleContainer);
            AddElement(moduleContainer, false, ratio);
        }

        private void AddElement(UIElement widget, bool IsSeperator, double ratio = 1.0)
        {
            GridUnitType GridType = IsSeperator ? GridUnitType.Auto : GridUnitType.Star;
            double MinSize = IsSeperator ? 0 : 40;
            if (Direction == Orientation.Horizontal)
            {
                Grid.SetColumn(widget, ModuleCount);
                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(ratio, GridType), MinWidth = MinSize });
            }
            else
            {
                Grid.SetRow(widget, ModuleCount);
                RowDefinitions.Add(new RowDefinition() { Height = new GridLength(ratio, GridType), MinHeight = MinSize });
            }
            Children.Add(widget);
        }
    }
}
