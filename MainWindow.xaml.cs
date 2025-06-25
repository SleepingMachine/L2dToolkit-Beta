using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using L2dToolkit_Beta.Pages;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.ApplicationSettings;
using Windows.UI.WindowManagement;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace L2dToolkit_Beta
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();

            //窗口材质拓展至标题栏
            ExtendsContentIntoTitleBar = true;
            contentFrame.Navigate(typeof(welcome_page));
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            //导航到设置页面
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(settings_page));
            }

            //导航到其他页面
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            switch((string)selectedItem.Tag)
            {
                case "TimerPage":
                    contentFrame.Navigate(typeof(timer_page));
                    break;

                case "WelcomePage":
                    contentFrame.Navigate(typeof(welcome_page));
                    break;
            }
        }
    }
}
