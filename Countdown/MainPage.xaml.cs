using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Popups;
using Countdown.Networking.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Countdown.Networking;
using Countdown.Networking.Results;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Task> TaskList = new ObservableCollection<Task>();
        public ObservableCollection<Task> SearchedTaskList { get; set; } = new ObservableCollection<Task>();

        private ServerConnection conn;

        public Page DisplayedPage;
        private ListViewer ListTaskView = new ListViewer();
        private CalendarViewer CalendarTaskView = new CalendarViewer();
        private SettingsViewer SettingsView = new SettingsViewer();
        private LoginViewer LoginView = new LoginViewer();
        

        private int uniqueID = 0;

        public MainPage()
        {
            this.InitializeComponent();
            DisplayedPage = ListTaskView;
            MyListBox.SelectedIndex = 0;
            ListTaskView.TaskList = TaskList;
            Bindings.Update();
            CreateTimer();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            conn = e.Parameter as ServerConnection;
            var newTasks = await conn.GetTasksForUser();
            var newInactiveTasks = await conn.GetInactiveTasksForUser();
            var TempList = new List<Task>();
            int maxID = -1;
            foreach (Task t in newTasks)
            {
                t.Subtasks = OrderList(t.Subtasks);
                TempList.Add(t);
                if (t.TaskId > maxID)
                {
                    maxID = t.TaskId;
                }
            }
            foreach (Task t in newInactiveTasks)
            {
                t.Subtasks = OrderList(t.Subtasks);
                TempList.Add(t);
                if (t.TaskId > maxID)
                {
                    maxID = t.TaskId;
                }
            }
            uniqueID = maxID;

            UserNameText.Text = AuthStorage.GetAuth().Value.Username;

            foreach (Task t in TempList.OrderBy(x => x.DueDate))
            {
                TaskList.Add(t);
            }

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
                TimeSpan rawValue = t.DueDate.Subtract(DateTime.Now);
                t.RemainingTime = new TimeSpan(rawValue.Days, rawValue.Hours, rawValue.Minutes, rawValue.Seconds);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewListBoxItem.IsSelected)
            {
                DisplayedPage = ListTaskView;
                ListTaskView.TaskList = SearchBar.Text.Length == 0 ? TaskList: SearchedTaskList;
                Bindings.Update();
                MyCommandBar.Visibility = Visibility.Visible;
                SearchBar.IsEnabled = true;
            }
            else if (CalendarViewListBoxItem.IsSelected)
            {
                DisplayedPage = CalendarTaskView;
                CalendarTaskView.TaskList = SearchBar.Text.Length == 0 ? TaskList : SearchedTaskList;
                Bindings.Update();
                MyCommandBar.Visibility = Visibility.Visible;
                SearchBar.IsEnabled = true;
            }
            else if (SettingsListBoxItem.IsSelected)
            {
                DisplayedPage = SettingsView;
                Bindings.Update();
                MyCommandBar.Visibility = Visibility.Collapsed;
                SearchBar.IsEnabled = false;
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
            var rawTime = selectedTime.Subtract(DateTime.Now);
            var addedTask = new Task
            {
                TaskId = uniqueID,
                Name = name,
                Description = description,
                DueDate = selectedTime,
                RemainingTime = new TimeSpan(rawTime.Days, rawTime.Hours, rawTime.Minutes, rawTime.Seconds)
            };
            uniqueID++;
            TaskList.Add(addedTask);
            await conn.CreateTask(addedTask);

            TaskList = OrderList(TaskList);
            if (SearchedTaskList.Count != 0)
            {
                SearchedTaskList.Add(addedTask);
                SearchedTaskList = OrderList(SearchedTaskList);
            }

            if (MyContentControl.Content is ListViewer)
            {
                ListTaskView.TaskList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
            }
            else if (MyContentControl.Content is CalendarViewer)
            {
                CalendarTaskView.TaskList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
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
                var selectedItemToRemove = TaskComboBox.SelectedIndex;
                if (selectedItemToRemove != -1)
                {
                    var SubtaskListBox = FindElementByName<ListBox>(MyContentControl, "SubtaskListBox");
                    var selectedSubtaskIndex = SubtaskListBox.SelectedIndex;
                    if (selectedSubtaskIndex == -1)
                    {
                        var result = await dialog.ShowAsync();
                        if (result != ContentDialogResult.Primary) return;
                        if (SearchedTaskList.Count != 0)
                        {
                            TaskList.Remove(SearchedTaskList[selectedItemToRemove]);
                            await conn.DeleteTask(SearchedTaskList[selectedItemToRemove]);
                            SearchedTaskList.RemoveAt(selectedItemToRemove);
                            ListTaskView.TaskList = SearchedTaskList;
                        }
                        else
                        {
                            await conn.DeleteTask(TaskList[selectedItemToRemove]);
                            TaskList.RemoveAt(selectedItemToRemove);
                            ListTaskView.TaskList = TaskList;
                        }
                    }
                    else
                    {
                        RemoveSubtask(selectedItemToRemove, selectedSubtaskIndex);
                    }
                }
                else
                {
                    var error = new MessageDialog("No Task selected to delete", "ERROR");
                    await error.ShowAsync();
                }
            }
            else if (MyContentControl.Content is CalendarViewer)
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "DayTaskListBox");
                int selectedItemToRemove = TaskComboBox.SelectedIndex;
                if (selectedItemToRemove != -1)
                {
                    var currentList = CalendarTaskView.DaysTasksList;

                    var TaskToRemove = currentList[selectedItemToRemove];

                    var result = await dialog.ShowAsync();
                    if (result != ContentDialogResult.Primary) return;
                    await conn.DeleteTask(TaskToRemove);
                    if (SearchedTaskList.Count != 0)
                    {
                        TaskList.Remove(TaskToRemove);
                        SearchedTaskList.Remove(TaskToRemove);
                        CalendarTaskView.TaskList = SearchedTaskList;
                    }
                    else
                    {
                        TaskList.Remove(TaskToRemove);
                        CalendarTaskView.TaskList = TaskList;
                    }
                }
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

        private async void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyContentControl.Content is ListViewer)
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "TaskListBox");
                int selectedItemToComplete = TaskComboBox.SelectedIndex;
                if (selectedItemToComplete != -1)
                {
                    var SubtaskListBox = FindElementByName<ListBox>(MyContentControl, "SubtaskListBox");
                    var selectedSubtaskIndex = SubtaskListBox.SelectedIndex;
                    var currentList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
                    if (selectedSubtaskIndex == -1)
                    { 
                        if (!currentList[selectedItemToComplete].IsCompleted)
                        {
                            await conn.MarkTaskAsCompleted(currentList[selectedItemToComplete]);
                        }
                        else
                        {
                            await conn.MarkTaskAsNotCompleted(currentList[selectedItemToComplete]);
                        }

                        ListTaskView.TaskList = new ObservableCollection<Task>();
                        ListTaskView.TaskList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
                        TaskComboBox.SelectedIndex = selectedItemToComplete;
                    }
                    else
                    {
                        var subtaskList = currentList[selectedItemToComplete].Subtasks;
                        var selectedSubtask = subtaskList[selectedSubtaskIndex];
                        if (!selectedSubtask.IsCompleted)
                        {
                            await conn.MarkTaskAsCompleted(selectedSubtask);
                        }
                        else
                        {
                            await conn.MarkTaskAsNotCompleted(selectedSubtask);
                        }

                        ListTaskView.TaskList = new ObservableCollection<Task>();
                        ListTaskView.TaskList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
                        TaskComboBox.SelectedIndex = selectedItemToComplete;
                        SubtaskListBox.SelectedIndex = selectedSubtaskIndex;
                    }
                    ListTaskView.SelectedTask = new Task {Name = "filler"};
                    ListTaskView.SelectedTask = currentList[selectedItemToComplete];
                    SubtaskListBox.SelectedIndex = selectedSubtaskIndex;
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("No Task selected to Complete.", "ERROR");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                var TaskComboBox = FindElementByName<ListBox>(MyContentControl, "DayTaskListBox");
                int selectedItemToComplete = TaskComboBox.SelectedIndex;
                if (selectedItemToComplete != -1)
                {
                    var currentList = CalendarTaskView.DaysTasksList;
                    if (!currentList[selectedItemToComplete].IsCompleted)
                    {
                        await conn.MarkTaskAsCompleted(currentList[selectedItemToComplete]);
                    }
                    else
                    {
                        await conn.MarkTaskAsNotCompleted(currentList[selectedItemToComplete]);
                    }

                    CalendarTaskView.TaskList = new ObservableCollection<Task>();
                    CalendarTaskView.TaskList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
                    TaskComboBox.SelectedIndex = selectedItemToComplete;
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("No Task selected to Complete.", "ERROR");
                    await dialog.ShowAsync();
                }
            }
        }

        private void SearchBar_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchedTaskList = new ObservableCollection<Task>();
            foreach (Task t in TaskList)
            {
                if (t.Name.ToLower().Contains(sender.Text.ToLower()))
                {
                    SearchedTaskList.Add(t);
                }
            }
            if (MyContentControl.Content is ListViewer)
            {
                ListTaskView.TaskList = SearchedTaskList;
            }
            else if (MyContentControl.Content is CalendarViewer)
            {
                CalendarTaskView.TaskList = SearchedTaskList;
            }

        }

        private async void AddSubtaskButton_Click(object sender, RoutedEventArgs e)
        {
            var TaskComboBox = MyContentControl.Content is ListViewer
                ? FindElementByName<ListBox>(MyContentControl, "TaskListBox")
                : FindElementByName<ListBox>(MyContentControl, "DayTaskListBox");
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
                var rawTime = selectedTime.Subtract(DateTime.Now);
                var addedTask = new Task
                {
                    TaskId = uniqueID,
                    Name = name,
                    Description = description,
                    DueDate = selectedTime,
                    RemainingTime = new TimeSpan(rawTime.Days, rawTime.Hours, rawTime.Minutes, rawTime.Seconds)
                };
                uniqueID++;
                if (MyContentControl.Content is ListViewer)
                {
                    var updatedSubtaskList = SearchedTaskList.Count == 0
                        ? new Task[TaskList[selectedItem].Subtasks.Length + 1]
                        : new Task[SearchedTaskList[selectedItem].Subtasks.Length + 1];

                    var currentList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;

                    for (int i = 0; i < currentList[selectedItem].Subtasks.Length; i++)
                    {
                        updatedSubtaskList[i] = currentList[selectedItem].Subtasks[i];
                    }

                    updatedSubtaskList[currentList[selectedItem].Subtasks.Length] = addedTask;
                    await conn.CreateSubTask(addedTask, currentList[selectedItem]);
                    currentList[selectedItem].Subtasks = OrderList(updatedSubtaskList);

                    ListTaskView.TaskList = currentList;
                }
                else if (MyContentControl.Content is CalendarViewer)
                {
                    var currentList = CalendarTaskView.DaysTasksList;
                    var updatedSubtaskList = new Task[currentList[selectedItem].Subtasks.Length + 1];

                    for (int i = 0; i < currentList[selectedItem].Subtasks.Length; i++)
                    {
                        updatedSubtaskList[i] = currentList[selectedItem].Subtasks[i];
                    }

                    updatedSubtaskList[currentList[selectedItem].Subtasks.Length] = addedTask;
                    await conn.CreateSubTask(addedTask, currentList[selectedItem]);
                    TaskList[TaskList.IndexOf(currentList[selectedItem])].Subtasks = updatedSubtaskList;
                    currentList[selectedItem].Subtasks = OrderList(updatedSubtaskList);
                    if (SearchedTaskList.Count != 0)
                    {
                        SearchedTaskList[SearchedTaskList.IndexOf(currentList[selectedItem])].Subtasks =
                            updatedSubtaskList;
                        CalendarTaskView.TaskList = SearchedTaskList;
                    }
                    else
                    {
                        CalendarTaskView.TaskList = TaskList;
                    }
                }
            }
            else
            {
                var error = new MessageDialog("No Task selected to add Subtask to.", "ERROR");
                await error.ShowAsync();
            }
        }

        private async void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var TaskComboBox = MyContentControl.Content is ListViewer
                ? FindElementByName<ListBox>(MyContentControl, "TaskListBox")
                : FindElementByName<ListBox>(MyContentControl, "DayTaskListBox");
            int selectedItem = TaskComboBox.SelectedIndex;
            if (selectedItem != -1)
            {
                bool Invalid = true;
                var currentList = new ObservableCollection<Task>();
                var SubtaskIndex = -1;
                if (MyContentControl.Content is ListViewer)
                {
                    currentList = SearchedTaskList.Count != 0 ? SearchedTaskList : TaskList;
                    var SubtaskListBox = FindElementByName<ListBox>(MyContentControl, "SubtaskListBox");
                    SubtaskIndex = SubtaskListBox.SelectedIndex;
                }
                else
                {
                    currentList = CalendarTaskView.DaysTasksList;
                }

                if (SubtaskIndex == -1)
                {
                    String name = currentList[selectedItem].Name, description = currentList[selectedItem].Description;
                    DateTime date = currentList[selectedItem].DueDate.Date;
                    TimeSpan time = currentList[selectedItem].DueDate.TimeOfDay;

                    while (Invalid)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Edit Task",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Visibility = Visibility.Visible
                        };

                        var stack = new StackPanel();
                        var nameTextBox = new TextBox {Text = name};
                        var descriptionTextBox = new TextBox {Text = description};
                        var selectedDueDate = new DatePicker {Date = date};
                        var selectedDueTime = new TimePicker {Time = time};
                        stack.Children.Add(new TextBlock {Text = "Task Name"});
                        stack.Children.Add(nameTextBox);
                        stack.Children.Add(new TextBlock {Text = "Description"});
                        stack.Children.Add(descriptionTextBox);
                        stack.Children.Add(new TextBlock {Text = "Due Date"});
                        stack.Children.Add(selectedDueDate);
                        stack.Children.Add(selectedDueTime);

                        dialog.Content = stack;
                        dialog.PrimaryButtonText = "Update";
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
                    var rawTime = selectedTime.Subtract(DateTime.Now);
                    var editedTask = new Task
                    {
                        TaskId = currentList[selectedItem].TaskId,
                        Name = name,
                        Description = description,
                        DueDate = selectedTime,
                        Subtasks = currentList[selectedItem].Subtasks,
                        RemainingTime = new TimeSpan(rawTime.Days, rawTime.Hours, rawTime.Minutes, rawTime.Seconds)
                    };

                    await conn.EditTask(editedTask);
                    if (MyContentControl.Content is ListViewer)
                    {
                        var currentItem = currentList[selectedItem];

                        if (SearchedTaskList.Count != 0)
                        {
                            SearchedTaskList[selectedItem] = editedTask;
                            TaskList[TaskList.IndexOf(currentItem)] = editedTask;
                            ListTaskView.TaskList = OrderList(SearchedTaskList);
                            SearchBar_TextChanged(SearchBar, null);
                        }
                        else
                        {
                            TaskList[selectedItem] = editedTask;
                            ListTaskView.TaskList = OrderList(TaskList);
                        }
                    }
                    else if (MyContentControl.Content is CalendarViewer)
                    {
                        var selectedElement = CalendarTaskView.DaysTasksList[selectedItem];

                        TaskList[TaskList.IndexOf(selectedElement)] = editedTask;

                        if (SearchedTaskList.Count != 0)
                        {
                            SearchedTaskList[SearchedTaskList.IndexOf(selectedElement)] = editedTask;
                            CalendarTaskView.TaskList = OrderList(SearchedTaskList);
                            SearchBar_TextChanged(SearchBar, null);
                        }
                        else
                        {
                            CalendarTaskView.TaskList = OrderList(TaskList);
                        }
                    }
                }
                else
                {
                    EditSubtask(selectedItem, SubtaskIndex);
                }
            }
            else
            {
                var error = new MessageDialog("No Task selected to add Subtask to.", "ERROR");
                await error.ShowAsync();
            }
            
        }

        public async void RemoveSubtask(int itemToRemove, int subtaskToRemove)
        {
            var dialog = new ContentDialog { Title = "Delete" };
            TextBlock text = new TextBlock { Text = "Are you sure you want to delete this Subtask?" };
            dialog.Content = text;
            dialog.PrimaryButtonText = "Yes";
            dialog.SecondaryButtonText = "Cancel";

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            var currentList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
            Task TaskToEdit = currentList[itemToRemove];
            Task[] newSubtaskList = new Task[TaskToEdit.Subtasks.Length - 1];
            for (int i = 0; i < TaskToEdit.Subtasks.Length; i++)
            {
                if (i < subtaskToRemove)
                {
                    newSubtaskList[i] = TaskToEdit.Subtasks[i];
                }
                else if (i > subtaskToRemove)
                {
                    newSubtaskList[i - 1] = TaskToEdit.Subtasks[i];
                }
            }

            await conn.DeleteTask(TaskToEdit.Subtasks[subtaskToRemove]);

            TaskToEdit.Subtasks = newSubtaskList;

            ListTaskView.TaskList = currentList;
        }

        public async void EditSubtask(int taskIndex, int subtaskIndex)
        {
            bool Invalid = true;
            var currentList = SearchedTaskList.Count == 0 ? TaskList : SearchedTaskList;
            var selectedItem = currentList[taskIndex];
            var selectedSubtask = selectedItem.Subtasks[subtaskIndex];
            String name = selectedSubtask.Name, description = selectedSubtask.Description;
            DateTime date = selectedSubtask.DueDate.Date;
            TimeSpan time = selectedSubtask.DueDate.TimeOfDay;

            while (Invalid)
            {
                var dialog = new ContentDialog
                {
                    Title = "Edit Subtask",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Visibility = Visibility.Visible
                };

                var stack = new StackPanel();
                var nameTextBox = new TextBox { Text = name };
                var descriptionTextBox = new TextBox { Text = description };
                var selectedDueDate = new DatePicker { Date = date };
                var selectedDueTime = new TimePicker { Time = time };
                stack.Children.Add(new TextBlock { Text = "Subtask Name" });
                stack.Children.Add(nameTextBox);
                stack.Children.Add(new TextBlock { Text = "Description" });
                stack.Children.Add(descriptionTextBox);
                stack.Children.Add(new TextBlock { Text = "Due Date" });
                stack.Children.Add(selectedDueDate);
                stack.Children.Add(selectedDueTime);

                dialog.Content = stack;
                dialog.PrimaryButtonText = "Update";
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
            var rawTime = selectedTime.Subtract(DateTime.Now);
            var editedTask = new Task
            {
                TaskId = selectedSubtask.TaskId,
                Name = name,
                Description = description,
                DueDate = selectedTime,
                RemainingTime = new TimeSpan(rawTime.Days, rawTime.Hours, rawTime.Minutes, rawTime.Seconds)
            };

            await conn.EditTask(editedTask);

            if (SearchedTaskList.Count != 0)
            {
                selectedSubtask = editedTask;
                TaskList[TaskList.IndexOf(selectedItem)].Subtasks[subtaskIndex] = editedTask;
                TaskList[TaskList.IndexOf(selectedItem)].Subtasks =
                    OrderList(TaskList[TaskList.IndexOf(selectedItem)].Subtasks);
                ListTaskView.TaskList = SearchedTaskList;
            }
            else
            {
                TaskList[TaskList.IndexOf(selectedItem)].Subtasks[subtaskIndex] = editedTask;
                TaskList[TaskList.IndexOf(selectedItem)].Subtasks = OrderList(TaskList[TaskList.IndexOf(selectedItem)].Subtasks);
                ListTaskView.TaskList = TaskList;
            }           
        }

        public ObservableCollection<Task> OrderList(ObservableCollection<Task> currentList)
        {
            var TempList = new List<Task>();

            foreach (Task t in currentList)
            {
                TempList.Add(t);
            }
            currentList.Clear();
            foreach (Task t in TempList.OrderBy(x => x.DueDate))
            {
                currentList.Add(t);
            }
            return currentList;
        }

        public Task[] OrderList(Task[] currentList)
        {
            var TempList = new List<Task>();

            foreach (Task t in currentList)
            {
                TempList.Add(t);
            }
            currentList = new Task[currentList.Length];
            int i = 0;
            foreach (Task t in TempList.OrderBy(x => x.DueDate))
            {
                currentList[i] = t;
                i++;
            }
            return currentList;
        }
    }

}
