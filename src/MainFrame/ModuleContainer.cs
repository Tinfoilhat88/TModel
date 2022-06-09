using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using TModel.Modules;
using TModel.MainFrame.Modules;
using static TModel.ColorConverters;
using TModel.MainFrame.Widgets;

namespace TModel
{
    /// <summary>
    /// Contains multiple instances of <see cref="ModuleBase"/> that can be switched using the tab bar
    /// </summary>
    public sealed class ModuleContainer : ContentControl
    {
        private TabControl TabBar = new TabControl()
        {
            Background = Theme.BackDark,
            Margin = new Thickness(0),
            Padding = new Thickness(2),
            BorderThickness = new Thickness(0),
        };

        private Grid OverlayPanel = new Grid();

        public ModulePanel? ParentPanel;

        public List<ModuleBase> Modules { get; } = new List<ModuleBase>();

        public ModuleContainer(ModuleBase Base, ModulePanel? parent = null)
        {
            ParentPanel = parent;
            AddModule(Base);
            Modules.Add(Base);
            OverlayPanel.Children.Add(TabBar);
            Content = OverlayPanel;
#if false // Splits container upon right clicking
            MouseRightButtonDown += (sender, args) =>
            {
                ModuleContainer NewContainer = (ModuleContainer)Activator.CreateInstance(typeof(ModuleContainer), new ItemPreviewModule(), null)!;
                if (ParentPanel != null)
                    ParentPanel.MakeSeperator(this, NewContainer);
            };
#endif
        }

        public ModuleContainer(ModuleBase Base, bool UseTabBar)
        {
            if (!UseTabBar)
            {
                Content = Base;
            }
            else
            {
                ParentPanel = null;
                AddModule(Base);
                Modules.Add(Base);
                OverlayPanel.Children.Add(TabBar);
                Content = OverlayPanel;
            }
#if false // Splits container upon right clicking
            MouseRightButtonDown += (sender, args) =>
            {
                ModuleContainer NewContainer = (ModulaeContainer)Activator.CreateInstance(typeof(ModuleContainer), new ItemPreviewModule(), null)!;
                if (ParentPanel != null)
                    ParentPanel.MakeSeperator(this, NewContainer);
            };
#endif
        }

        public void AddModule(ModuleBase module)
        {
            Modules.Add(module);
            TabBar.Items.Add(new TabItem() { Header = new TextBlock() { Text = module.ModuleName, FontSize = 20 }, Content = module });
        }

        public bool TryShowModule<T>()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (typeof(T) == Modules[i].GetType())
                {
                    TabBar.SelectedIndex = i - 1;
                    return true;
                }
            }

            return false;
        }
    }
}
