using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Countdown.Networking.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewer : Page
    {
        public ObservableCollection<Task> TaskList
        {
            get { return taskList; }
            set
            {
                taskList = value;
                Bindings.Update();
            }
        }

        private ObservableCollection<Task> taskList = new ObservableCollection<Task>();

        private Task selectedTask;

        public Task SelectedTask
        {
            get { return selectedTask; }
            set { selectedTask = value;
                Bindings.Update(); }
        }

        private int previousSelection = -1;

        public ListViewer()
        {
            this.InitializeComponent();
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
            if (SelectedTask != null)
            {
                TimeSpan rawValue = SelectedTask.DueDate.Subtract(DateTime.Now);
                SelectedTask.RemainingTime = new TimeSpan(rawValue.Days, rawValue.Hours, rawValue.Minutes,
                    rawValue.Seconds);
                Bindings.Update();
            }
        }

        private void TaskListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                SelectedTask = TaskList[selectedIndex];
                Bindings.Update();    
            }
        }

        private void SubtaskListBox_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentSelection = SubtaskListBox.SelectedIndex;
            if (currentSelection != -1 && previousSelection == currentSelection)
            {
                    SubtaskListBox.SelectedIndex = -1;
                    previousSelection = -1;
                    return;
            }
            previousSelection = currentSelection;
        }
    }
}
