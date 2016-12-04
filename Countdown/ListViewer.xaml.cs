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

        public Task SelectedTask;

        private int previousSelection = -1, previousSubtaskSelection = -1;

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
                if (SelectedTask.RemainingTime.TotalSeconds > 0)
                {
                    TimeSpan rawValue = SelectedTask.DueDate.Subtract(DateTime.Now);
                    SelectedTask.RemainingTime = new TimeSpan(rawValue.Days, rawValue.Hours, rawValue.Minutes,
                        rawValue.Seconds);
                }
                else
                {
                    SelectedTask.RemainingTime = new TimeSpan(0,0,0,0);
                }
                Bindings.Update();
            }
        }

        private void TaskListBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (selectedIndex == previousSelection)
                {
                    SelectedTask = new Task
                    {
                        Name = " ",
                        Description = " ",
                        Subtasks = new Task[0],
                        DueDate = DateTime.MinValue,
                        RemainingTime = new TimeSpan(0, 0, 0, 0)
                    };
                    TaskListBox.SelectedIndex = -1;
                    previousSelection = -1;
                }
                else
                {
                    SelectedTask = TaskList[selectedIndex];
                    previousSelection = selectedIndex;
                    Bindings.Update();
                }
            }
            else
            {
                SelectedTask = new Task
                {
                    Name = " ",
                    Description = " ",
                    Subtasks = new Task[0],
                    DueDate = DateTime.MinValue,
                    RemainingTime = new TimeSpan(0,0,0,0)
                };
                previousSelection = -1;
                Bindings.Update();
            }
        }

        private void SubtaskListBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            int selectedIndex = SubtaskListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (selectedIndex == previousSubtaskSelection)
                {
                    SubtaskListBox.SelectedIndex = -1;
                    previousSubtaskSelection = -1;
                }
                else
                {
                    previousSubtaskSelection = selectedIndex;
                }
            }
            else
            {
                previousSubtaskSelection = -1;
                Bindings.Update();
            }
        }
    }
}
