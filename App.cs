// ONLY FOR DEBUGGING
#define GENERATE_MODULES // When false window only shows ItemPreviewModule    default: true
// #define NO_WINDOW // True if there should NOT be a window                     defualt: false

using System;
using System.Windows;
using TModel.Modules;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using System.Windows.Threading;
using CUE4Parse.UE4.Assets;
using CUE4Parse.FN.Exports.FortniteGame;
using TModel.MainFrame.Modules;
using static TModel.ColorConverters;
using Serilog;
using Serilog.Configuration;
using CUE4Parse.Compression;
using System.Windows.Controls;

namespace TModel
{
    public class App : Application
    {
        // Runs the given action on the UI thread.
        public static void RunOnUI(Action action)
        {
            Current?.Dispatcher.Invoke(action, DispatcherPriority.Background);
        }
#if GENERATE_MODULES && !NO_WINDOW
        static ModulePanel modulePanel = new ModulePanel();
#endif

        public static Window Window { get; } = new Window();

        public static DefaultFileProvider FileProvider { set; get; } = null;

        // Trys to find the type of module, if found then shows it
        public static void ShowModule<T>()
        {
#if GENERATE_MODULES && !NO_WINDOW
            modulePanel.TryShowModule<T>();
#endif
        }
        public static bool IsValidGameDirectory(string? path)
        {
            return path != null && Directory.Exists(path);
        }

        [STAThread]
        public static void Main()
        {
            Directory.CreateDirectory(Preferences.StorageFolder);

#if GENERATE_MODULES && !NO_WINDOW
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MySink()
                .CreateLogger();
#endif
            Preferences.Read();
#if NO_WINDOW
            DebugTools.DirectLoad();
#endif
#if GENERATE_MODULES && !NO_WINDOW
            if (IsValidGameDirectory(Preferences.GameDirectory))
            {
                FileProvider = new DefaultFileProvider(Preferences.GameDirectory, SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_1));
                App.FileProvider.Initialize();
            }
#endif
#if NO_WINDOW
            // Makes sure the storage folder exists
            Directory.CreateDirectory(Preferences.StorageFolder);
#endif
#if GENERATE_MODULES && !NO_WINDOW
#if false
            ModuleContainer Module_Left = new ModuleContainer(new DirectoryModule(), ModulePanel);
#else
            ModuleContainer Module_Left = new ModuleContainer(new GameContentModule(), modulePanel);
#endif
            ModuleContainer Module_Right = new ModuleContainer(new FileManagerModule(), modulePanel);
#endif
#if !NO_WINDOW
            ObjectTypeRegistry.RegisterEngine(typeof(UFortItemDefinition).Assembly);
#endif

#if false // Preload data
            FileManagerModule.LoadGame();
#endif
// FileProvider.GetFilesInPath(@"FortniteGame/Content/Athena/Items");
            var app = new App();
#if !NO_WINDOW
            Window.Title = "TModel";
            Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Window.ResizeMode = ResizeMode.CanResize;
            Window.MinWidth = 400;
            Window.MinHeight = 200;
            Window.Background = HexBrush("#15162a");
            Window.Loaded += (sender, args) =>
            {
#endif
#if GENERATE_MODULES && !NO_WINDOW
                Window.Content = modulePanel;
#else
#if !NO_WINDOW
                Window.Content = new ItemPreviewModule();
#endif
#endif
#if !NO_WINDOW

            };
            app.MainWindow = Window;
            Window.Show();
#endif
#if GENERATE_MODULES && !NO_WINDOW
#if false
            Module_Right.AddModule(new ObjectViewerModule());
#endif
            Module_Left.AddModule(new SettingsModule());
            Module_Right.AddModule(new ItemPreviewModule());


            modulePanel.AddModule(Module_Left, 0.56);
            modulePanel.AddModule(Module_Right, 0.44);

            if (string.IsNullOrWhiteSpace(Preferences.GameDirectory))
            {
                ShowModule<SettingsModule>();
            }

            modulePanel.MakeSeperator(Module_Right, new ModuleContainer(new LoggerModule(), false), new GridLength(150, GridUnitType.Pixel));

            app.Run();
#endif
        }
    }

#if !NO_WINDOW
    public static class MySinkExtensions
    {
        public static LoggerConfiguration MySink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new LoggerSink(formatProvider));
        }
    }
#endif
}
