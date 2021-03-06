﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Countdown.Networking.Serialization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarViewer : Page
    {
        private ObservableCollection<Task> taskList = new ObservableCollection<Task>();

        public ObservableCollection<Task> TaskList
        {
            get { return taskList; }
            set { taskList = value;
                MyCalendar_SelectedDatesChanged(MyCalendar, null);
                Bindings.Update();
                }
        }

        private int previousSelection = -1;

        public ObservableCollection<Task> DaysTasksList { get; set; } = new ObservableCollection<Task>();

        public string NoTasksMessage = "No Tasks";

        public CalendarViewer()
        {
            this.InitializeComponent();
            
        }

        private void MyCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            var selectedDays = sender.SelectedDates;
            if (selectedDays.Count == 0)
            {
                DaysTasksList = TaskList;
            }
            else
            {
                List<DateTime> dateTimes = new List<DateTime>();
                DaysTasksList = new ObservableCollection<Task>();
                foreach (DateTimeOffset dt in selectedDays)
                {
                    dateTimes.Add(dt.DateTime.Date);
                }

                foreach (Task t in TaskList)
                {
                    if (dateTimes.Contains(t.DueDate.Date))
                    {
                        DaysTasksList.Add(t);
                    }
                }
            }

            NoTasksMessage = DaysTasksList.Count == 0 ? "No Tasks" : " ";
            Bindings.Update();

        }

        private void DayTaskListBox_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            int currentSelection = DayTaskListBox.SelectedIndex;
            if (currentSelection != -1)
            {
                if (previousSelection == currentSelection)
                {
                    DayTaskListBox.SelectedIndex = -1;
                    previousSelection = -1;
                    return;
                }
            }
            previousSelection = currentSelection;
        }
    }
}
