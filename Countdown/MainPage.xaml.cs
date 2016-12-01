﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Countdown.Networking.Serialization;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Task> TaskList { get; } = new ObservableCollection<Task>();
        public ObservableCollection<Task> SearchedTaskList { get; set; } = new ObservableCollection<Task>();

        public MainPage()
        {
            this.InitializeComponent();
            MyContentControl.Content = new ListViewer();
            CreateTimer();
        }

        public void CreateTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void timer_Tick(object sender, object e)
        {
            foreach (Task t in TaskList)
            {
                t.RemainingTime = t.DueDate.Subtract(DateTime.Now);
            }
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
                MyContentControl.Content = new ListViewer(TaskList);
                MyCommandBar.Visibility = Visibility.Visible;
            }
            if (CalendarViewListBoxItem.IsSelected)
            {
                MyContentControl.Content = new CalendarViewer();
                MyCommandBar.Visibility = Visibility.Visible;
            }
            else if (SettingsListBoxItem.IsSelected)
            {
                MyContentControl.Content = new SettingsViewer();
                MyCommandBar.Visibility = Visibility.Collapsed;
            }
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            MyListBox.SelectedItem = null;
            //if (MyFrame.CanGoBack)
            {
                //MyFrame.GoBack();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            bool Invalid = true;
            String name = "", description = "";
            DateTime date = new DateTime();
            TimeSpan time = new TimeSpan();

            while (Invalid)
            {
                var dialog = new ContentDialog
                {
                    Title = "Add Task",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Visibility = Visibility.Visible
                };

                var stack = new StackPanel();
                var nameTextBox = new TextBox();
                var descriptionTextBox = new TextBox();
                var selectedDueDate = new DatePicker();
                var selectedDueTime = new TimePicker();
                stack.Children.Add(new TextBlock {Text = "Task Name"});
                stack.Children.Add(nameTextBox);
                stack.Children.Add(new TextBlock {Text = "Description"});
                stack.Children.Add(descriptionTextBox);
                stack.Children.Add(new TextBlock {Text = "Due Date"});
                stack.Children.Add(selectedDueDate);
                stack.Children.Add(selectedDueTime);

                dialog.Content = stack;
                dialog.PrimaryButtonText = "Add";
                dialog.SecondaryButtonText = "Cancel";

                var result = await dialog.ShowAsync();

                if (result != ContentDialogResult.Primary) return;

                Invalid = nameTextBox.Text == "" || descriptionTextBox.Text == "";
                if (Invalid)
                {
                    MessageDialog error = new MessageDialog("Invalid Task Name or Description", "ERROR");
                    var ok = error.ShowAsync();
                }
                else
                {
                    name = nameTextBox.Text;
                    description = descriptionTextBox.Text;
                    date = selectedDueDate.Date.DateTime;
                    time = selectedDueTime.Time;
                }
            }

            var selectedTime = new DateTime(date.Date.Year, date.Date.Month,
                date.Date.Day, time.Hours, time.Minutes,
                time.Seconds);
            var addedTask = new Task
            {
                TaskId = 0,
                Name = name,
                Description = description,
                DueDate = selectedTime,
                RemainingTime = selectedTime.Subtract(DateTime.Now)
            };
            TaskList.Add(addedTask);
            if (MyContentControl.Content is ListViewer)
            {
                MyContentControl.Content = new ListViewer(TaskList);
            }
            else if (MyContentControl.Content is CalendarViewer)
            {
                MyContentControl.Content = new CalendarViewer(TaskList);
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog {Title="Delete"};
            TextBlock text = new TextBlock {Text = "Are you sure you want to delete this task?"};
            dialog.Content = text;
            dialog.PrimaryButtonText = "Yes";
            dialog.SecondaryButtonText = "Cancel";



            if (MyContentControl.Content is ListViewer)
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "TaskListBox");
                int selectedItemToRemove = TaskComboBox.SelectedIndex;
                if (selectedItemToRemove != -1)
                {
                    var result = await dialog.ShowAsync();
                    if (result != ContentDialogResult.Primary) return;
                    TaskList.RemoveAt(selectedItemToRemove);
                }
                else
                {
                    var error = new MessageDialog("No Task selected to delete", "ERROR");
                    await error.ShowAsync();
                }
                MyContentControl.Content = new ListViewer(TaskList);
            }
        }

        public T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyContentControl.Content is ListViewer)
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "TaskListBox");
                int selectedItemToComplete = TaskComboBox.SelectedIndex;
                TaskList[selectedItemToComplete].IsCompleted = true;
            }
        }

        private void SearchBar_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            foreach(Task t in TaskList)
            {
                if (t.Name.ToLower().Contains(sender.Text.ToLower()))
                {
                    SearchedTaskList.Add(t);
                }
            }
            MyContentControl.Content = new ListViewer(SearchedTaskList);
            SearchedTaskList = new ObservableCollection<Task>();
        }

        private async void AddSubtaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyContentControl.Content is ListViewer)
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "TaskListBox");
                int selectedItem = TaskComboBox.SelectedIndex;
                if (selectedItem != -1)
                {
                    bool Invalid = true;
                    String name = "", description = "";
                    DateTime date = new DateTime();
                    TimeSpan time = new TimeSpan();

                    while (Invalid)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Add Subtask",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Visibility = Visibility.Visible
                        };

                        var stack = new StackPanel();
                        var nameTextBox = new TextBox();
                        var descriptionTextBox = new TextBox();
                        var selectedDueDate = new DatePicker();
                        var selectedDueTime = new TimePicker();
                        stack.Children.Add(new TextBlock { Text = "Subtask Name" });
                        stack.Children.Add(nameTextBox);
                        stack.Children.Add(new TextBlock { Text = "Description" });
                        stack.Children.Add(descriptionTextBox);
                        stack.Children.Add(new TextBlock { Text = "Due Date" });
                        stack.Children.Add(selectedDueDate);
                        stack.Children.Add(selectedDueTime);

                        dialog.Content = stack;
                        dialog.PrimaryButtonText = "Add";
                        dialog.SecondaryButtonText = "Cancel";

                        var result = await dialog.ShowAsync();

                        if (result != ContentDialogResult.Primary) return;

                        Invalid = nameTextBox.Text == "" || descriptionTextBox.Text == "";
                        if (Invalid)
                        {
                            MessageDialog error = new MessageDialog("Invalid Subtask Name or Description", "ERROR");
                            var ok = error.ShowAsync();
                        }
                        else
                        {
                            name = nameTextBox.Text;
                            description = descriptionTextBox.Text;
                            date = selectedDueDate.Date.DateTime;
                            time = selectedDueTime.Time;
                        }
                    }

                    var selectedTime = new DateTime(date.Date.Year, date.Date.Month,
                        date.Date.Day, time.Hours, time.Minutes,
                        time.Seconds);
                    var addedTask = new Task
                    {
                        TaskId = 0,
                        Name = name,
                        Description = description,
                        DueDate = selectedTime,
                        RemainingTime = selectedTime.Subtract(DateTime.Now)
                    };

                    Task[] updatedSubtaskList = new Task[TaskList[selectedItem].Subtasks.Length + 1];
                    for(int i = 0; i < TaskList[selectedItem].Subtasks.Length; i++)
                    {
                        updatedSubtaskList[i] = TaskList[selectedItem].Subtasks[i];
                    }
                    updatedSubtaskList[TaskList[selectedItem].Subtasks.Length] = addedTask;
                    TaskList[selectedItem].Subtasks = updatedSubtaskList;
                    MyContentControl.Content = new ListViewer(TaskList);
                }
                else
                {
                    var error = new MessageDialog("No Task selected to add Subtask to.", "ERROR");
                    await error.ShowAsync();
                }
            }
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }

}
