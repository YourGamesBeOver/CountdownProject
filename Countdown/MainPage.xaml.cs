using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Countdown.Networking.Serialization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Task> TaskList { get; } = new ObservableCollection<Task>();

        public MainPage()
        {
            this.InitializeComponent();

            MyFrame.Navigate(typeof(ListViewer), TaskList);
            
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = MyListBox.SelectedItem == null ? AppViewBackButtonVisibility.Collapsed : AppViewBackButtonVisibility.Visible;

            currentView.BackRequested += backButton_Tapped;

            if (ListViewListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(ListViewer), TaskList);
                MyCommandBar.Visibility = Visibility.Visible;
            }
            if(CalendarViewListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(CalendarViewer));
                MyCommandBar.Visibility = Visibility.Visible;
            }
            else if(SettingsListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(SettingsViewer));
                MyCommandBar.Visibility = Visibility.Collapsed;
            }
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            MyListBox.SelectedItem = null;
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog { Title = "Add Task", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Visible };

            var stack = new StackPanel();
            var NameTextBox = new TextBox();
            var DescriptionTextBox = new TextBox();
            var SelectedDueDate = new DatePicker();
            var SelectedDueTime = new TimePicker();
            var parentTaskBox = new ComboBox();
            stack.Children.Add(new TextBlock { Text = "Task Name" });
            stack.Children.Add( NameTextBox );
            stack.Children.Add(new TextBlock { Text = "Description" });
            stack.Children.Add(DescriptionTextBox);
            stack.Children.Add(new TextBlock { Text = "Due Date" });
            stack.Children.Add(SelectedDueDate);
            stack.Children.Add(SelectedDueTime);

            parentTaskBox.Items.Add("None");
            foreach (Task t in taskList)
            {
                parentTaskBox.Items.Add(t.Name);
            }

            stack.Children.Add(new TextBlock {Text = "Parent Task"});
            stack.Children.Add(parentTaskBox);

            dialog.Content = stack;
            dialog.PrimaryButtonText = "Add";
            dialog.SecondaryButtonText = "Cancel";

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;

            var selectedTime = new DateTime(selectedDueDate.Date.Year, selectedDueDate.Date.Month,
                selectedDueDate.Date.Day, selectedDueTime.Time.Hours, selectedDueTime.Time.Minutes,
                selectedDueTime.Time.Seconds);
            var addedTask = new Task
            {
                TaskId = 0,
                Name = nameTextBox.Text,
                Description = descriptionTextBox.Text,
                DueDate = selectedTime
            };
            TaskList.Add(addedTask);
            MyFrame.Navigate(typeof(ListViewer), TaskList);
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog { Title = "Remove Task", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Visible };

            if (taskList.Count == 0)
            {
                var text = new TextBlock { Text = "No Tasks to delete" };

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
