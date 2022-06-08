using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TModel.Export;
using TModel.MainFrame.Widgets;
using static TModel.ColorConverters;

namespace TModel.Modules
{
    class ItemPreviewModule : ModuleBase
    {
        public override string ModuleName => "Item Preview";

        ExportPreviewInfo Item;

        public static int[]? SelectedStyles = null;

        public ItemPreviewModule()
        {
            Grid Root = new Grid();

            Root.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            Root.RowDefinitions.Add(new RowDefinition());

            Grid LowerPanel = new Grid();
            Grid.SetRow(LowerPanel, 1);
            Root.Children.Add(LowerPanel);
#if SHOW_RELATED_COSMETICS
            LowerPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80, GridUnitType.Pixel)});
#endif
            LowerPanel.ColumnDefinitions.Add(new ColumnDefinition());

            Grid LowerRightPanel = new Grid() { Background = Theme.BackDark };
            LowerRightPanel.RowDefinitions.Add(new RowDefinition());
            LowerRightPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
#if SHOW_RELATED_COSMETICS
            Grid.SetColumn(LowerRightPanel, 1);
#else
            Grid.SetColumn(LowerRightPanel, 0);
#endif
            LowerPanel.Children.Add(LowerRightPanel);

            StackPanel StyleSetNamesPanel = new StackPanel() 
            { 
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            CScrollViewer StylesScroller = new CScrollViewer();
            StackPanel StylesPanel = new StackPanel()
            {
                Margin = new Thickness(0,20,0,0),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            StylesScroller.Content = StylesPanel;
            CScrollViewer StylePickerScroller = new CScrollViewer();
            Grid StylePickerPanel = new Grid() { VerticalAlignment = VerticalAlignment.Top };
            StylePickerPanel.Margin = new Thickness(20, 0, 0, 0);
            StylePickerScroller.Content = StylePickerPanel;
            StylePickerPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            StylePickerPanel.ColumnDefinitions.Add(new ColumnDefinition());
            StylePickerPanel.Children.Add(StyleSetNamesPanel);
            Grid.SetColumn(StylesScroller, 1);
            // StylePickerPanel.Children.Add(StylesScroller);
            LowerRightPanel.Children.Add(StylePickerScroller);

            Border ButtonsBorder = new Border() 
            {
                Background = HexBrush("#222228"),
                BorderBrush = HexBrush("#595959"),
                BorderThickness = new Thickness(0,2,0,0),
                MaxHeight = 80,
                MinHeight = 60,
                MinWidth = 120
            };
            Grid ButtonsGrid = new Grid();
#if SHOW_ALL_BUTTONS
            ButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
#endif
            ButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid ButtonsRowGrid = new Grid();
            ButtonsRowGrid.RowDefinitions.Add(new RowDefinition());
            ButtonsRowGrid.RowDefinitions.Add(new RowDefinition());
#if SHOW_ALL_BUTTONS
            CButton SaveButton = new CButton("Save to Library", 20) { Margin = new Thickness(6, 6, 2, 2) };
            CButton DirectoryButton = new CButton("Show in Directory", 20) { Margin = new Thickness(6, 2, 2, 6) };
#endif
            CButton ExportButton = new CButton("Export", 50, () =>
            {
                if (Item != null)
                {
#if !DEBUG
                    try
                    {
                        
#endif
                        Log.Information($"\n======== Exporting: {Item.Name} ========\n");
                        GameContentModule.CurrentExporter.GetBlenderExportInfo(Item.Package, SelectedStyles).Save();
#if !DEBUG
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Failed to export:\n" + e.ToString());
                    }
#endif
                }
                else
                {
                    Log.Warning("No item selected");
                }
            }) { Margin = new Thickness(2, 6, 2, 6) };
#if SHOW_ALL_BUTTONS
            CButton ShowModelButton = new CButton("Show in\nModel Viewer", 20) { Margin = new Thickness(2, 6, 6, 6) };
#endif
#if SHOW_ALL_BUTTONS
            Grid.SetRow(SaveButton, 0);
            Grid.SetRow(DirectoryButton, 1);
            ButtonsRowGrid.Children.Add(SaveButton);
            ButtonsRowGrid.Children.Add(DirectoryButton);

            Grid.SetColumn(ShowModelButton, 2);
            ButtonsGrid.Children.Add(ShowModelButton);
#endif

            Grid.SetColumn(ExportButton, 1);
            ButtonsGrid.Children.Add(ExportButton);



            ButtonsGrid.Children.Add(ButtonsRowGrid);
            ButtonsBorder.Child = ButtonsGrid;

            Grid.SetRow(ButtonsBorder, 1);
            LowerRightPanel.Children.Add(ButtonsBorder);

#if SHOW_RELATED_COSMETICS
            Border RelatedCosmeticsBorder = new Border();
            RelatedCosmeticsBorder.Background = HexBrush("#27282d");
            RelatedCosmeticsBorder.BorderBrush = HexBrush("#515156");
            RelatedCosmeticsBorder.BorderThickness = new Thickness(0,0,2,0);
            LowerPanel.Children.Add(RelatedCosmeticsBorder);

            CScrollViewer RelatedCosmeticsScroller = new CScrollViewer();
            StackPanel RelatedCosmeticsStack = new StackPanel();
            RelatedCosmeticsScroller.Content = RelatedCosmeticsStack;
            RelatedCosmeticsBorder.Child = RelatedCosmeticsScroller;
            RelatedCosmeticsStack.Margin = new Thickness(1);

            for (int i = 0; i < 20; i++)
            {
                Border CosmeticBorder = new Border();
                CosmeticBorder.SizeChanged += (sender, args) =>
                {
                    CosmeticBorder.Height = CosmeticBorder.Width;
                };
                
                CosmeticBorder.Margin = new Thickness(3);
                CosmeticBorder.Background = Theme.BackNormal;
                CosmeticBorder.BorderBrush = Theme.BorderNormal;
                CosmeticBorder.BorderThickness = new Thickness(2);

                Image CosmeticIcon = new Image() { Source = IconImage };
                CosmeticBorder.Child = CosmeticIcon;

                RelatedCosmeticsStack.Children.Add(CosmeticBorder);
            }
#endif
            Border TopBorder = new Border();

            Grid TopGrid = new Grid();
            TopBorder.Child = TopGrid;
            TopBorder.BorderThickness = new Thickness(0,0,0,2);
            TopBorder.BorderBrush = HexBrush("#454652");
            TopBorder.Background = HexBrush("#1a1b25");

            TopGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140, GridUnitType.Pixel) });
            TopGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Viewbox DisplayNameViewBox = new Viewbox()
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            CTextBlock DisplayName = new CTextBlock("", 40) 
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DisplayNameViewBox.Child = DisplayName;

            Grid TopRightGrid = new Grid();

            Grid TopLowerRightGrid = new Grid();

            TopLowerRightGrid.ColumnDefinitions.Add(new ColumnDefinition());
            TopLowerRightGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            Grid.SetRow(TopLowerRightGrid, 1);
            TopRightGrid.Children.Add(TopLowerRightGrid);
            Viewbox DescriptionViewBox = new Viewbox()
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment= VerticalAlignment.Center,
                MaxHeight = 20
            };
            CTextBlock Description = new CTextBlock("", 20) { TextWrapping = TextWrapping.Wrap };
            // CTextBlock Introduced = new CTextBlock("Chapter 3, Season 1", 20);
            // Grid.SetColumn(Introduced, 1);
            DescriptionViewBox.Child = Description;
            TopLowerRightGrid.Children.Add(DescriptionViewBox);
            // TopLowerRightGrid.Children.Add(Introduced);

            TopRightGrid.RowDefinitions.Add(new RowDefinition());
            TopRightGrid.RowDefinitions.Add(new RowDefinition());
            TopRightGrid.Children.Add(DisplayNameViewBox);
            TopRightGrid.Margin = new Thickness(0,0,10,10);
            Grid.SetColumn(TopRightGrid, 1);
            TopGrid.Children.Add(TopRightGrid);
            Image PreviewIcon = new Image();

            Root.Children.Add(TopBorder);
            TopGrid.Children.Add(PreviewIcon);
            Content = Root;

            GameContentModule.SelectionChanged += (ExportPreviewInfo item) =>
            {
                Item = item;
                StylePickerPanel.Children.Clear();

                DisplayName.Text = item.Name;
                Description.Text = item.FileName ?? "Missing \'FileName\'";
                if (item.PreviewIcon.TryGet_BitmapImage(out BitmapImage Source))
                {
                    PreviewIcon.Source = Source;
                }
                if (item.Styles != null)
                {
                    SelectedStyles = new int[item.Styles.Count];

                    for (int i = 0; i < item.Styles.Count; i++)
                    {
                        ExportPreviewSet style = item.Styles[i];
                        CTextBlock NameText = new CTextBlock(style.Name, 20)
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };

                        StylePickerPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        Grid.SetRow(NameText, i);
                        StylePickerPanel.Children.Add(NameText);


                        StyleSetWidget SetWidget = new StyleSetWidget(style, i);
                        Grid.SetRow(SetWidget, i);
                        Grid.SetColumn(SetWidget, 1);
                        StylePickerPanel.Children.Add(SetWidget);
                    }
                }
                else
                {
                    StylePickerPanel.Children.Add(new CTextBlock("NO STYLES", 40)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    });
                }
            };
        }

        // Only contains options not name
        public class StyleSetWidget : ContentControl
        {
            public StyleOptionWidget SelectedOption;

            public StyleSetWidget(ExportPreviewSet style, int index)
            {
                WrapPanel StyleSetOptionsPanel = new WrapPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(3),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                Content = StyleSetOptionsPanel;

                for (int i = 0; i < style.Options.Count; i++)
                {
                    StyleOptionWidget OptionWidget = new StyleOptionWidget(style.Options[i], this, index, i);
                    if (i == 0)
                    {
                        SelectedOption = OptionWidget;
                        OptionWidget.Select();
                    }
                    StyleSetOptionsPanel.Children.Add(OptionWidget);
                }
            }
        }

        public class StyleOptionWidget : ContentControl
        {
            public StyleSetWidget Owner;

            public bool IsSelected => Owner.SelectedOption == this;

            int InnerIndex;
            int OuterIndex;

            Border StyleOptionBorder = new Border()
            {
                BorderBrush = Theme.BorderNormal,
                Background = Theme.BackNormal,
                BorderThickness = new Thickness(3),
                Width = 80,
                Height = 80,
                Margin = new Thickness(5,20,5,5),
            };



            public StyleOptionWidget(ExportPreviewOption option, StyleSetWidget owner, int outerIndex, int innerIndex)
            {
                Owner = owner;
                OuterIndex = outerIndex;
                InnerIndex = innerIndex;

                // Hover FX
                MouseEnter += (sender, args) => StyleOptionBorder.BorderBrush = IsSelected ? Theme.BorderSelected : Theme.BorderHover;
                MouseLeave += (sender, args) => StyleOptionBorder.BorderBrush = IsSelected ? Theme.BorderSelected : Theme.BorderNormal;

                MouseLeftButtonDown += (sender, args) => Select();

                StyleOptionBorder.ToolTip = new CTooltip(option.Name);
                if (option.Icon is TextureRef VTexture)
                {
                    if (VTexture.TryGet_BitmapImage(out BitmapImage bitmapImage))
                    {
                        Image StyleOptionImage = new Image() { Source = bitmapImage };
                        StyleOptionBorder.Child = StyleOptionImage;
                    }
                }
                else
                {
                    Viewbox viewbox = new Viewbox() { Margin = new Thickness(5) };
                    CTextBlock cTextBlock = new CTextBlock(option.Name);
                    viewbox.Child = cTextBlock;
                    StyleOptionBorder.Child = viewbox;
                }

                Content = StyleOptionBorder;
            }

            public void Select()
            {
                Owner.SelectedOption.Deselect();
                SelectedStyles[OuterIndex] = InnerIndex;
                Owner.SelectedOption = this;
                StyleOptionBorder.BorderBrush = Theme.BorderSelected;
            }

            void Deselect()
            {
                StyleOptionBorder.BorderBrush = Theme.BorderNormal;
            }
        }
    }
}
