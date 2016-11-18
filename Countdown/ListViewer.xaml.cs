using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewer : Page
    {

        private ObservableCollection<Task> taskList = new ObservableCollection<Task>();

        public ObservableCollection<Task> TaskList
        {
            get { return taskList; }
        }


        public ListViewer()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler<object>(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = e.Parameter as ObservableCollection<Task>;
            if (list != null)
            {
                taskList = list;
            }
        }

        private void timer_Tick(object sender, object e)
        {
            foreach (Task t in taskList)
            {
                t.RemainingTime = t.DueDate.Subtract(DateTime.Now);
            }
        }

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            while (DetailsStackPanel.Children.Count > 0)
            {
                DetailsStackPanel.Children.RemoveAt(0);
            }

            if (selectedIndex != -1)
            {
                string nameText = "Task Name: " + taskList.ElementAt(selectedIndex).Name;
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = nameText,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                string descriptionText = "Description: " + taskList.ElementAt(selectedIndex).Description;
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = descriptionText,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                string dueDateText = "Due Date: " + taskList.ElementAt(selectedIndex).DueDate;
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = dueDateText,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = "-----------------------------",
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                if (taskList.ElementAt(selectedIndex).Subtasks.Count == 0)
                {
                    DetailsStackPanel.Children.Add(new TextBlock
                    {
                        Text = "No Subtasks",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                }
                else
                {
                    DetailsStackPanel.Children.Add(new TextBlock
                    {
                        Text = "Subtasks:",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    foreach (Task t in taskList.ElementAt(selectedIndex).Subtasks)
                    {
                        string subtaskInfo = t.Name + ", Due: " + t.DueDate;
                        DetailsStackPanel.Children.Add(new TextBlock
                        {
                            Text = subtaskInfo,
                            HorizontalAlignment = HorizontalAlignment.Center
                        });
                    }
                }
            }

        }
    }
}
