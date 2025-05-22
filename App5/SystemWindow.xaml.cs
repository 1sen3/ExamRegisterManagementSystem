using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using App5.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App5
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankWindow1 : Window
    {
        public static BlankWindow1 current;
        public BlankWindow1()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar=true;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            current = this;
            nav.IsPaneOpen = false;
        }
        private void nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                navframe.Navigate(typeof(SettingsPage));
            }
            else
            {
                string selected = args.SelectedItemContainer.Tag.ToString();
                switch (selected)
                {
                    case "HomePage":
                        navframe.Navigate(typeof(HomePage));
                        break;
                    case "ExamListPage":
                        navframe.Navigate(typeof(ExamListPage));
                        break;
                    case "MyExamPage":
                        navframe.Navigate(typeof(MyExamPage));
                        break;
                    case "MyInfoPage":
                        navframe.Navigate(typeof(MyInfoPage));
                        break;
                }
            }
        }
        private void nav_loaded(object sender,RoutedEventArgs e)
        {
            var homeItem=nav.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item=>item.Tag.ToString() == "HomePage");
            if (homeItem != null)
            {
                nav.SelectedItem = homeItem;
            }
        }
    }
}
