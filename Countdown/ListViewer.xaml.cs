using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Countdown.Networking.Serialization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewer : Page
    {
        public ObservableCollection<Task> TaskList { get; private set; } = new ObservableCollection<Task>();


        public ListViewer()
        {
            this.InitializeComponent();
        }

        public ListViewer(ObservableCollection<Task> tasks)
        {
            this.InitializeComponent();
            TaskList = tasks;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = e.Parameter as ObservableCollection<Task>;
            if (list != null)
            {
                TaskList = list;
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
                string nameText = "Task Name: " + TaskList[selectedIndex].Name;
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = nameText,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                string descriptionText = "Description: " + TaskList[selectedIndex].Description;
                DetailsStackPanel.Children.Add(new TextBlock
                {
                    Text = descriptionText,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                string dueDateText = "Due Date: " + TaskList[selectedIndex].DueDate;
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

                if (TaskList[selectedIndex].Subtasks.Length == 0)
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
                    if (TaskList[selectedIndex].Subtasks == null) return;
                    foreach (Task t in TaskList[selectedIndex].Subtasks)
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
