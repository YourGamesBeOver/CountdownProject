namespace Countdown.Networking {
    public class AuthStorage {

        /// <summary>
        /// returns true if there is a username and password stored
        /// </summary>
        /// <returns></returns>
        public static bool LoggedIn()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            var value = settings["loggedin"];
            if (value is bool) return (bool) value;
            settings["loggedin"] = false;
            return false;
        }

        /// <summary>
        /// Clears the stored username and password (ie when the user clicks 'log out')
        /// </summary>
        public static void LogOut()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            settings.Remove("username");
            settings.Remove("password");
            settings["loggedin"] = false;
        }

        /// <summary>
        /// stores the given user information
        /// </summary>
        /// <param name="auth"></param>
        public static void LogIn(UserAuth auth)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            settings["username"] = auth.Username;
            settings["password"] = auth.Password;
            settings["loggedin"] = true;
        }

        public static UserAuth? GetAuth()
        {
            if (!LoggedIn()) return null;
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            return new UserAuth {Username = (string) settings["username"], Password = (string) settings["password"]};
        }
    }
}
