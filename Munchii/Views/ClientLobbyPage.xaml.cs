using System;
using System.Linq;
using Xamarin.Forms;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Database.Query;
using System.Threading.Tasks;
using Munchii.Services;
using Xamarin.Essentials;


namespace Munchii
{
    public partial class ClientLobbyPage : ContentPage
    {
        private string roomCode;
        private string clientUserId;

        // Subscription to the QuizStarted field
        private IDisposable quizStartedSubscription;

        private Func<bool> timerAction;

        private void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(roomCode);
        }

        public ClientLobbyPage(string roomCode, string clientUserId)
        {
            InitializeComponent();

            this.roomCode = roomCode;
            this.clientUserId = clientUserId;

            NavigationPage.SetHasNavigationBar(this, false);

            RoomCodeLabel.Text = $"Room Code: {roomCode}";

            // Start the LastSeenUpdater
            LastSeenUpdater.Instance.Start(roomCode, clientUserId);

            timerAction = TimerTick;

            Device.StartTimer(TimeSpan.FromSeconds(1), timerAction);

            Digit1.Text = roomCode[0].ToString();
            Digit2.Text = roomCode[1].ToString();
            Digit3.Text = roomCode[2].ToString();
            Digit4.Text = roomCode[3].ToString();
        }

        private bool quizAlreadyStarted = false;

        private bool TimerTick()
        {
            Task.Run(() => CheckNullRoom());

            App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .OnceAsync<User>()
                .ContinueWith(task => {
                    var users = task.Result;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (users != null)
                        {
                            UserCountLabel.Text = $"User Count: {users.Count}";
                        }
                    });
                });

            App.Database
            .Child("rooms")
            .Child(roomCode)
            .Child("QuizStarted")
            .OnceSingleAsync<bool>()
            .ContinueWith(task => {
                if (task.IsFaulted)
                {
                    // Log the error if there is one
                    Console.WriteLine(task.Exception.ToString());
                    return;
                }

                var quizStarted = task.Result;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (quizStarted == true && !quizAlreadyStarted)
                    {
                        quizAlreadyStarted = true;
                        await Navigation.PushAsync(new DealBreakerPage(roomCode, clientUserId));
                        Console.WriteLine("Quiz started, navigating to QuizPage...");
                    }
                });
            }).ConfigureAwait(false);  // Add ConfigureAwait here

            return !quizAlreadyStarted; // Stop the timer if the quiz has started
        
        }


        private async Task CheckNullRoom()
        {

            var firebaseRooms = await App.Database
                                        .Child("rooms")
                                        .Child(roomCode)
                                        .OnceAsync<Room>();

            var room = firebaseRooms?.FirstOrDefault();
            if (room == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                });
            }
        }

        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            await RemoveUser();
            await Navigation.PopAsync();
        }

        private async Task RemoveUser()
        {
            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .Child(clientUserId)
                .DeleteAsync();
        }
    }
}
