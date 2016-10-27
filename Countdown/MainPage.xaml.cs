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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<HamOption> hamOptions = new List<HamOption>
            {
                new HamOption() { Title = "Profile" },
                new HamOption() { Title = "Blog Posts" },
                new HamOption() { Title = "Stats" },
                new HamOption() { Title = "Settings" },

            };

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
            if(ListViewListBoxItem.IsSelected)
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
    }

    public class HamOption
    {

        public string Title { get; set; }

        public Type PageType { get; set; }

        public string IconFile { get; set; }


    }
}
