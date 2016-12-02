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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewer : Page
    {
        public ObservableCollection<Task> TaskList { get; private set; } = new ObservableCollection<Task>();
        public Task SelectedTask = new Task();

        public ListViewer()
        {
            this.InitializeComponent();
        }

        public ListViewer(ObservableCollection<Task> tasks)
        {
            this.InitializeComponent();
            TaskList = tasks;
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
            if (TaskListBox.SelectedIndex != -1)
            {
                TimeSpan rawValue = SelectedTask.DueDate.Subtract(DateTime.Now);
                SelectedTask.RemainingTime = new TimeSpan(rawValue.Hours, rawValue.Minutes, rawValue.Seconds);
                Bindings.Update();
            }
        }

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                SelectedTask = TaskList[selectedIndex];
                Bindings.Update();
            }

        }
    }
}
