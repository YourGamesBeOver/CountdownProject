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
    public sealed partial class ListViewer : Page
    {
        public ObservableCollection<Task> TaskList { get; private set; } = new ObservableCollection<Task>();

        public ListViewer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var list = e.Parameter as ObservableCollection<Task>;
            if (list != null)
            {
                TaskList = list;
            }
        }
    }
}
