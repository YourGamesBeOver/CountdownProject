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

        public CalendarViewer()
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

        private void MyCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            var selectedDays = args.AddedDates;

        }
    }
}
