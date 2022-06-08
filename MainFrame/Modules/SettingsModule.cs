using CUE4Parse.UE4.Versions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TModel.MainFrame.Widgets;
using TModel.Modules;
using static TModel.ColorConverters;

namespace TModel.MainFrame.Modules
{
    public class SettingsModule : ModuleBase
    {
        public static Action ReadSettings;

        public override string ModuleName => "Settings";

        Grid SettingsPanel = new Grid()
        {
            
            Margin = new Thickness(20)
        };

        // Options
        CTextBox GameDirectoryText = new CTextBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
        };
        CheckBox AutoLoadOnStartup = new CheckBox()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            RenderTransform = new ScaleTransform(1.8,1.8)
        };

        CButton UpdateData = new CButton("Update")
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };

        private TextBlock UnsavedPreferenceText = new TextBlock()
        {
            Visibility = Visibility.Hidden,
            FontSize = 20,
            Foreground = HexBrush("#ff5c5c"),
            Text = "Unsaved preferences!"
        };

        private TextBlock DirectoryErrorText = new TextBlock()
        {
            Visibility = Visibility.Hidden,
            HorizontalAlignment = HorizontalAlignment.Right,
            FontSize = 15,
            Foreground = HexBrush("#ff5c5c"),
            Text = DirectoryDoesNotExist,
        };

        const string DirectoryDoesNotExist = "Directory does not exist!";
        const string NoFortniteFilesInDirectory = "Directory does not contain Fortnite files!";

        private void CheckForUnsaved() 
        {
            if (AutoLoadOnStartup.IsChecked != Preferences.AutoLoad || Preferences.GameDirectory != GameDirectoryText.Text)
            {
                UnsavedPreferenceText.Visibility = Visibility.Visible;
            }
            else
            {
                UnsavedPreferenceText.Visibility = Visibility.Hidden;
            }
        }

        public SettingsModule() : base()
        {
            GameDirectoryText.TextChanged += (sender, args) =>
            {
                if (Directory.Exists(GameDirectoryText.Text))
                {
                    if (!Directory.EnumerateFiles(GameDirectoryText.Text, "*.utoc").Any())
                    {
                        DirectoryErrorText.Text = NoFortniteFilesInDirectory;
                        DirectoryErrorText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        DirectoryErrorText.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    DirectoryErrorText.Text = DirectoryDoesNotExist;
                    DirectoryErrorText.Visibility = Visibility.Visible;
                }
                CheckForUnsaved();
            };
            AutoLoadOnStartup.Click += (sender, args) => CheckForUnsaved();

            SettingsPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            SettingsPanel.ColumnDefinitions.Add(new ColumnDefinition());
            // 
            Grid Root = new Grid()
            {
                Background = Theme.BackDark,
            };
            Root.RowDefinitions.Add(new RowDefinition());
            Root.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(100) });

            Border ButtonPanelBorder = new Border()
            {
                Background = HexBrush("#1d1d25"),
                Padding = new Thickness(10),
                BorderThickness = new Thickness(0,3,0,0),
                BorderBrush = HexBrush("#3b3b3b")
            };

            StackPanel ButtonPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            Grid.SetRow(ButtonPanelBorder, 1);
            Root.Children.Add(ButtonPanelBorder);
            ButtonPanelBorder.Child = ButtonPanel;
            CButton SaveButton = new CButton("Save", 60, () =>
            {
                Preferences.GameDirectory = GameDirectoryText.Text;
                Preferences.AutoLoad = AutoLoadOnStartup.IsChecked ?? false;
                Preferences.Save();
                UnsavedPreferenceText.Visibility = Visibility.Hidden;
            });

            ButtonPanel.Children.Add(SaveButton);

            AddOption("Game Directory", GameDirectoryText);
            AddOption("Auto Load Upon Startup", AutoLoadOnStartup);
            AddOption("Update AES and Mappings", UpdateData);

            UpdateData.Click += () =>
            {
                Log.Information("Updated Mappings");
                App.FileProvider.LoadMappings();
                Log.Information("Updated AES keys");
                FileManagerModule.LoadAES(true);
                Log.Information("Restart required for changes to take affect");
            };

            ReadSettings += () =>
            {
                Preferences.Read();
                GameDirectoryText.Text = string.IsNullOrWhiteSpace(Preferences.GameDirectory) ? null : Preferences.GameDirectory;
                AutoLoadOnStartup.IsChecked = Preferences.AutoLoad;
            };

            Root.Children.Add(UnsavedPreferenceText);
            Root.Children.Add(DirectoryErrorText);

            Root.Children.Add(SettingsPanel);
            Content = Root;
            ReadSettings();
        }

        public void AddOption(string name, UIElement input)
        {
            CTextBlock NameText = new CTextBlock(name, 20)
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0,0,40,0),
            };
            Grid.SetRow(NameText, SettingsPanel.RowDefinitions.Count);
            Grid.SetRow(input, SettingsPanel.RowDefinitions.Count);
            Grid.SetColumn(input, 1);

            SettingsPanel.Children.Add(NameText);
            SettingsPanel.Children.Add(input);
            SettingsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
        }
    }
}
