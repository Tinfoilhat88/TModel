using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using CUE4Parse.UE4.Vfs;
using CUE4Parse.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TModel.MainFrame.Modules;
using TModel.MainFrame.Widgets;
using TModel.Sorters;
using static TModel.ColorConverters;

namespace TModel.Modules
{
    // Manages loading of VFS
    public class FileManagerModule : ModuleBase
    {
        public override string ModuleName => "File Manager";

        public static bool HasLoaded;
        public static bool IsLoading;

        bool FirstTimeShown = true;

        public static Action FilesLoaded;

        public FileManagerModule() : base()
        {
            Grid Root = new Grid();
            Content = Root;
            Root.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.0, GridUnitType.Star) });

            StackPanel ButtonPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            ButtonPanel.HorizontalAlignment = HorizontalAlignment.Center;
            ButtonPanel.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetRow(ButtonPanel, 0);

            Root.Children.Add(ButtonPanel);

            Root.Background = Theme.BackDark;
            CButton LoadButton = new CButton("Load", 40);
            LoadButton.Click += () => 
            {
                if (!IsLoading)
                {
                    if (!HasLoaded)
                    {
                        LoadGameFiles();
                    }
                    else
                    {
                        Log.Information("Won't load again, Already loaded.");
                    }
                }
                else
                {
                    Log.Information("Already loading files.");
                }
            };

            LoadButton.Height = 90;
            LoadButton.Width = 180;

            ButtonPanel.Children.Add(LoadButton);

            Preferences.Changed += () =>
            {
                if (App.IsValidGameDirectory(Preferences.GameDirectory))
                {
                    if (App.FileProvider == null)
                    {
                        App.FileProvider = new DefaultFileProvider(Preferences.GameDirectory, SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_1));
                        App.FileProvider.Initialize();
                    }
                    LoadFiles(App.FileProvider._unloadedVfs.Keys);
                }
            };

            // App.FileProvider.UnloadedVfs.Sort(new NameSort()); 
            if (App.FileProvider != null)
                LoadFiles(App.FileProvider._unloadedVfs.Keys);

            this.Loaded += (sender, args) =>
            {
                if (FirstTimeShown)
                {
                    if (Preferences.AutoLoad && App.FileProvider != null)
                    {
                        Log.Information("Auto loading on startup");
                        LoadGameFiles();
                    }
                        
                    FirstTimeShown = false;
                }
            };
        }

        public static Exception? FailedReason;

        public void LoadGameFiles()
        {
            IsLoading = true;
            Task.Run(() =>
            {
                if (App.IsValidGameDirectory(Preferences.GameDirectory))
                {
                    if (App.FileProvider == null)
                    {
                        App.FileProvider = new DefaultFileProvider(Preferences.GameDirectory, SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_1));
                        App.FileProvider.Initialize();
                    }
                    FailedReason = InitilizeGame();
                }
                else
                {
                    Log.Warning(string.IsNullOrEmpty(Preferences.GameDirectory) ? "Please set the Game Directory in settings" : $"\'{Preferences.GameDirectory}\' is not a valid directory (Change this in settings then restart)");
                }
            }
            ).GetAwaiter().OnCompleted(() =>
            {
                if (FailedReason is not null)
                {
                    Log.Error("Cannot load Fortnite!");
                }
                else
                {
                    if (App.FileProvider != null)
                    {
                        if (App.FileProvider.MappingsForThisGame != null)
                        {
                            HasLoaded = true;
                            IsLoading = false;
                            List<IAesVfsReader> AllVFS = new List<IAesVfsReader>();
                            AllVFS.AddRange(App.FileProvider.MountedVfs);
                            AllVFS.AddRange(App.FileProvider._unloadedVfs.Keys);
                            AllVFS.Sort(NameSort.Global);
                            LoadFiles(AllVFS, true);
                            if (FilesLoaded != null)
                                FilesLoaded();
                            Log.Information("Finished loading");
                        }
                    }
                }
            });
        }

        public static Exception? InitilizeGame()
        {
            Log.Information("Submitting keys");
            LoadAES();
            App.FileProvider.LoadMappings();

            if (App.FileProvider.MappingsContainer is BenBotMappingsProvider benbotMappings && !benbotMappings.Successful)
            {
                return BenBotMappingsProvider.FailedMappingsException;
            }
            else
            {
                return null;
            }
        }

        public static void LoadAES(bool force = false)
        {
            // Submits AES keys
            AesKeys AesKeys;
            string JsonString = "";
            string AesKeysFile = Path.Combine(Preferences.StorageFolder, "AesKeys.json");
            if (File.Exists(AesKeysFile) && !force)
            {
                // Load aes keys from file
                JsonString = File.ReadAllText(AesKeysFile);
            }
            else
            {
                // Download aes keys from Fortnite API
                WebClient Client = new WebClient();
                JsonString = Client.DownloadString(@"https://fortnite-api.com/v2/aes");
                // Save the keys on disk
                File.WriteAllText(AesKeysFile, JsonString);
            }
            AesKeys = JsonConvert.DeserializeObject<MainAesKeys>(JsonString).data;
            // Main key

            App.FileProvider.SubmitKey(new FGuid(), new FAesKey("0xA9AA68E485AB1A9A1777F5BE7A02C35ECCD93FCCCCD147F2EA67BA2DA7D35430"));

            if (AesKeys.mainkey is not null)
            {
                App.FileProvider.SubmitKey(new FGuid(), new FAesKey(AesKeys.mainkey));
            }
            
            // Dynamic keys
            foreach (DynamicAesKey key in AesKeys.dynamicKeys)
                App.FileProvider.SubmitKey(new FGuid(key.pakGuid), new FAesKey(key.key));
        }

        struct MainAesKeys
        {
            public AesKeys data;
        }

        struct AesKeys
        {
            public string mainkey;
            public DynamicAesKey[] dynamicKeys;
        }

        struct DynamicAesKey
        {
            public string pakGuid;
            public string key;
        }

        private void LoadFiles(ICollection<IAesVfsReader> files, bool tryload = false)
        {

        }
    }

    public class FileManagerItem : ContentControl
    {
        // Needs name (and file size probs)
        public FileManagerItem(IAesVfsReader reader, bool tryload = false)
        {
            Border RootBorder = new Border()
            {
                Padding = new Thickness(5),
                Background = Theme.BackNormal,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top
            };

            MouseEnter += (sender, args) =>
            {
                RootBorder.Background = Theme.BackHover;
            };

            MouseLeave += (sender, args) =>
            {
                RootBorder.Background = Theme.BackNormal;
            };

            MouseLeftButtonUp += (sender, args) =>
            {
                RootBorder.Background = Theme.BackNormal;
            };

            CTextBlock FilesCountText = new CTextBlock(reader.FileCount.ToString())
            {
                TextAlignment = TextAlignment.Right,
                Width = 60,
                Margin = new Thickness(0, 0, 40, 0)
            };

            Grid MainPanel = new Grid();
            MainPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            MainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            RootBorder.Child = MainPanel;

            CTextBlock FileNameText = new CTextBlock(reader.Name)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            StackPanel DetailsPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            Grid.SetColumn(DetailsPanel, 1);
            MainPanel.Children.Add(FileNameText);

            DetailsPanel.Children.Add(FilesCountText);


            MainPanel.Children.Add(DetailsPanel);

            Content = RootBorder;
        }
    }
}
