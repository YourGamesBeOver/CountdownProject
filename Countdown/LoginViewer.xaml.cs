using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Countdown.Networking;
using Countdown.Networking.Results;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginViewer : Page
    {

        public LoginViewer()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            ServerConnection conn = new ServerConnection();
            conn.Connect(ResourceManager.Current.MainResourceMap.GetSubtree("Resources").GetValue("ServerURL", ResourceContext.GetForViewIndependentUse()).ValueAsString);
            var auth = new UserAuth {Password = PasswordTextBox.Password, Username = UserNameTextBox.Text};
            var loginResult = await conn.LogIn(auth);

            switch (loginResult)
            {
                case LoginResult.Success:
                    AuthStorage.LogIn(auth);
                    rootFrame.Navigate(typeof(MainPage), conn);
                    break;
                case LoginResult.BadParams:
                    MessageDialog error = new MessageDialog("Username or Password is Invalid", "ERROR");
                    await error.ShowAsync();
                    conn.Dispose();
                    break;
                case LoginResult.Error:
                    MessageDialog error2 = new MessageDialog("Unknown Error", "ERROR");
                    await error2.ShowAsync();
                    conn.Dispose();
                    break;
            } 
        }

        private async void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            ServerConnection conn = new ServerConnection();
            conn.Connect(ResourceManager.Current.MainResourceMap.GetSubtree("Resources").GetValue("ServerURL", ResourceContext.GetForViewIndependentUse()).ValueAsString);
            var auth = new UserAuth {Password = PasswordTextBox.Password, Username = UserNameTextBox.Text};
            var loginResult = await conn.CreateUser(auth);

            switch (loginResult)
            {
                case CreateUserResult.Success:
                    conn.Dispose();
                    LoginButton_OnClick(null,null);
                    break;
                case CreateUserResult.UsernameTaken:
                    MessageDialog error = new MessageDialog("Username taken.", "ERROR");
                    await error.ShowAsync();
                    conn.Dispose();
                    break;
                case CreateUserResult.Error:
                    MessageDialog error2 = new MessageDialog("Unknown Error", "ERROR");
                    await error2.ShowAsync();
                    conn.Dispose();
                    break;
            } 
        }
    }
}
