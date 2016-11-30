using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
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
        private ObservableCollection<Task> daysTasksList { get; } = new ObservableCollection<Task>();

        public CalendarViewer()
        {
            this.InitializeComponent();
            
        }

        public CalendarViewer(ObservableCollection<Task> newList)
        {
            taskList = newList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = e.Parameter as ObservableCollection<Task>;
            if (list != null)
            {
                taskList = list;
            }
        }

        private void MyCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            var selectedDays = args.AddedDates;
            List<DateTime> dateTimes = new List<DateTime>();
            foreach (DateTimeOffset dt in selectedDays)
            {
                dateTimes.Add(dt.DateTime.Date);
            }

            foreach (Task t in taskList)
            {
                if (dateTimes.Contains(t.DueDate.Date))
                {
                    daysTasksList.Add(t);
                } 
            }

        }
    }
}
