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
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private ObservableCollection<Task> taskList = new ObservableCollection<Task>();
        public ObservableCollection<Task> TaskList { get { return taskList; } }

        public MainPage()
        {
            this.InitializeComponent();

            myFrame.Navigate(typeof(ListViewer), taskList);
            
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
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
                myFrame.Navigate(typeof(ListViewer), taskList);
                MyCommandBar.Visibility = Visibility.Visible;
            }
            if(CalendarViewListBoxItem.IsSelected)
            {
                myFrame.Navigate(typeof(CalendarViewer));
                MyCommandBar.Visibility = Visibility.Visible;
            }
            else if(SettingsListBoxItem.IsSelected)
            {
                myFrame.Navigate(typeof(SettingsViewer));
                MyCommandBar.Visibility = Visibility.Collapsed;
            }
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            MyListBox.SelectedItem = null;
            if (myFrame.CanGoBack)
            {
                myFrame.GoBack();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog { Title = "Add Task", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment=VerticalAlignment.Center, Visibility = Visibility.Visible };

            var stack = new StackPanel();
            var NameTextBox = new TextBox();
            var DescriptionTextBox = new TextBox();
            var SelectedDueDate = new DatePicker();
            var SelectedDueTime = new TimePicker();
            stack.Children.Add(new TextBlock { Text = "Task Name" });
            stack.Children.Add( NameTextBox );
            stack.Children.Add(new TextBlock { Text = "Description" });
            stack.Children.Add(DescriptionTextBox);
            stack.Children.Add(new TextBlock { Text = "Due Date" });
            stack.Children.Add(SelectedDueDate);
            stack.Children.Add(SelectedDueTime);

            dialog.Content = stack;
            dialog.PrimaryButtonText = "Add";
            dialog.SecondaryButtonText = "Cancel";

            var result = await dialog.ShowAsync();

            switch(result)
            {
                case ContentDialogResult.Primary:
                    DateTime SelectedTime = new DateTime(SelectedDueDate.Date.Year, SelectedDueDate.Date.Month, SelectedDueDate.Date.Day, SelectedDueTime.Time.Hours, SelectedDueTime.Time.Minutes, SelectedDueTime.Time.Seconds);
                    Task addedTask = new Task(0,NameTextBox.Text, DescriptionTextBox.Text, SelectedTime);
                    taskList.Add(addedTask);
                    myFrame.Navigate(typeof(ListViewer), taskList);
                    break;
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog { Title = "Remove Task", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Visible };

            if (taskList.Count == 0)
            {
                var text = new TextBlock {Text = "No Tasks to delete"};

                dialog.Content = text;
                dialog.PrimaryButtonText = "OK";

                await dialog.ShowAsync();
            }
            else
            {
                var list = new ListBox();

                foreach (Task t in taskList)
                {
                    list.Items.Add(t.Name);
                }

                dialog.Content = list;
                dialog.PrimaryButtonText = "Delete";
                dialog.SecondaryButtonText = "Cancel";

                var result = await dialog.ShowAsync();

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (list.SelectedIndex != -1)
                        {
                            taskList.RemoveAt(list.SelectedIndex);
                        }
                        myFrame.Navigate(typeof(ListViewer), taskList);
                        break;
                }
            }
        }
    }

}
