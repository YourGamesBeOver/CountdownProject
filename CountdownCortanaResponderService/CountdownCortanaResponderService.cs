using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Countdown.Networking;
using Countdown.Networking.Results;
using AsyncTask = System.Threading.Tasks.Task;

namespace Countdown.CortanaResponderService
{
    public sealed class CountdownCortanaResponderService : IBackgroundTask
    {
        private BackgroundTaskDeferral _serviceDeferral;

        private ResourceMap _resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

        /// <summary>
        /// the service connection is maintained for the lifetime of a cortana session, once a voice command
        /// has been triggered via Cortana.
        /// </summary>
        VoiceCommandServiceConnection _voiceServiceConnection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails == null || triggerDetails.Name != "CountdownCortanaResponderService") return;
            try {
                _voiceServiceConnection =
                    VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                        triggerDetails);

                _voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                // GetVoiceCommandAsync establishes initial connection to Cortana, and must be called prior to any 
                // messages sent to Cortana. Attempting to use ReportSuccessAsync, ReportProgressAsync, etc
                // prior to calling this will produce undefined behavior.
                VoiceCommand voiceCommand = await _voiceServiceConnection.GetVoiceCommandAsync();


                // Depending on the operation (defined in AdventureWorks:AdventureWorksCommands.xml)
                // perform the appropriate command.
                switch (voiceCommand.CommandName) {
                    case "showNextCountdown":

                        await SendNextTaskMessage();
                        break;
                    default:
                        // As with app activation VCDs, we need to handle the possibility that
                        // an app update may remove a voice command that is still registered.
                        // This can happen if the user hasn't run an app since an update.
                        LaunchAppInForeground("Launching Countdown");
                        break;
                }
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
            }
        }

        private async AsyncTask SendNextTaskMessage()
        {
            await ShowProgressScreen("Let me check");
            using (var connection = new ServerConnection())
            {
                if (!await LogIn(connection)) return;
                var responseTask = await connection.GetNextCountdown();
                var userMessage = new VoiceCommandUserMessage();
                string message;
                if (responseTask == null)
                {
                    message = "I could not locate the task";
                }
                else
                {
                    message =
                        $"Your next task is {responseTask.Name}, it is due on {responseTask.DueDate}; in {responseTask.DueDate - DateTime.Now}";
                }
                userMessage.SpokenMessage = userMessage.DisplayMessage = message;
                var userResponse = VoiceCommandResponse.CreateResponse(userMessage);
                await _voiceServiceConnection.ReportSuccessAsync(userResponse);
            }
        }

        private async System.Threading.Tasks.Task<bool> LogIn(ServerConnection connection)
        {
            connection.Connect(_resourceMap.GetValue("ServerURL", ResourceContext.GetForViewIndependentUse()).ValueAsString);
            //connection.Connect("http://localhost:5000");

            if (!AuthStorage.LoggedIn()) {
                LaunchAppInForeground("You need to log in first");
                return false;
            }
            var auth = AuthStorage.GetAuth();
            if (auth == null) {
                LaunchAppInForeground("You need to log in first");
                return false;
            }
            var loginresult = await connection.LogIn(auth.Value);
            if (loginresult != LoginResult.Success) {
                LaunchAppInForeground("There was an error");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Show a progress screen. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        private async AsyncTask ShowProgressScreen(string message) {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await _voiceServiceConnection.ReportProgressAsync(response);
        }

        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        private async void LaunchAppInForeground(string spokenMessage) {
            var userMessage = new VoiceCommandUserMessage {SpokenMessage = spokenMessage, DisplayMessage = spokenMessage};

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await _voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            _serviceDeferral?.Complete();
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _serviceDeferral?.Complete();
        }
    }
}
