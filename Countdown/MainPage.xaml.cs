using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Countdown;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MyFrame.Navigate(typeof(ListViewer));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MyListBox.SelectedItem = null;
            MyFrame.Navigate(typeof(ListViewer));
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            if (MyListBox.SelectedItem == null)
            {
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            else
            {
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }

            currentView.BackRequested += backButton_Tapped;

            if (ListViewListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(ListViewer));
            }
            if(CalendarViewListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(CalendarViewer));
            }
            else if(SettingsListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(SettingsViewer));
            }
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            MyListBox.SelectedItem = null;
            MyFrame.Navigate(typeof(ListViewer));
        }
    }
}
