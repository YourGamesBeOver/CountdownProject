using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Countdown.Networking;

namespace Countdown
{

    public sealed partial class SettingsViewer : Page
    {
        public SettingsViewer()
        {
            this.InitializeComponent();
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthStorage.LogOut();
            var frame = Window.Current.Content as Frame;
            frame?.Navigate(typeof(LoginViewer));
        }
    }
}
