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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = e.Parameter as ObservableCollection<Task>;
            if (list != null)
            {
                taskList = list;
            }
        }

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TaskListBox.SelectedIndex;
            if (selectedIndex == -1)
            {
                DetailsStackPanel = new StackPanel();
            }
            else
            {
                string nameText = "Task Name: " + taskList.ElementAt(selectedIndex).Name;
                DetailsStackPanel.Children.Add(new TextBlock { Text = nameText, HorizontalAlignment = HorizontalAlignment.Center });
                string descriptionText = "Description: " + taskList.ElementAt(selectedIndex).Description;
                DetailsStackPanel.Children.Add(new TextBlock { Text = descriptionText, HorizontalAlignment = HorizontalAlignment.Center });
                string dueDateText = "Due Date: " + taskList.ElementAt(selectedIndex).DueDate;
                DetailsStackPanel.Children.Add(new TextBlock { Text = dueDateText, HorizontalAlignment = HorizontalAlignment.Center });
            }
        }
    }
}
