using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Firebase.Database;
using Firebase.Database.Query;
using Munchii.Services;

namespace Munchii
{
    public partial class ClientLobbyPage : ContentPage
    {
        private readonly string roomCode;
        private readonly string clientUserId;
        private bool quizAlreadyStarted = false;

        public ClientLobbyPage(string roomCode, string clientUserId)
        {
            InitializeComponent();

            // Initialize member variables
            this.roomCode = roomCode;
            this.clientUserId = clientUserId;

            // UI settings
            NavigationPage.SetHasNavigationBar(this, false);
            RoomCodeLabel.Text = $"Room Code: {roomCode}";
            InitializeRoomCodeDigits();

            // Update user's last seen time
            LastSeenUpdater.Instance.Start(roomCode, clientUserId);

            // Start timer for checking quiz start and other updates
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerTick);
        }

        // Initialize the room code digits display
        private void InitializeRoomCodeDigits()
        {
            Digit1.Text = roomCode[0].ToString();
            Digit2.Text = roomCode[1].ToString();
            Digit3.Text = roomCode[2].ToString();
            Digit4.Text = roomCode[3].ToString();
        }

        // Timer tick function
        private bool TimerTick()
        {
            UpdateUserCount();
            CheckQuizStarted();
            Task.Run(CheckNullRoom);

            return !quizAlreadyStarted; // Stop the timer if the quiz has started
        }

        // Update the user count label
        private async void UpdateUserCount()
        {
            var users = await App.Database
                                  .Child("rooms")
                                  .Child(roomCode)
                                  .Child("Users")
                                  .OnceAsync<User>();

            Device.BeginInvokeOnMainThread(() =>
            {
                if (users != null)
                {
                    UserCountLabel.Text = $"User Count: {users.Count}";
                }
            });
        }

        // Check if the quiz has started
        private async void CheckQuizStarted()
        {
            var quizStarted = await App.Database
                                      .Child("rooms")
                                      .Child(roomCode)
                                      .Child("QuizStarted")
                                      .OnceSingleAsync<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (quizStarted && !quizAlreadyStarted)
                {
                    quizAlreadyStarted = true;
                    await Navigation.PushAsync(new DealBreakerPage(roomCode, clientUserId));
                    Console.WriteLine("Quiz started, navigating to QuizPage...");
                }
            });
        }

        // Check if room is null and navigate back if it is
        private async Task CheckNullRoom()
        {
            var room = (await App.Database
                              .Child("rooms")
                              .Child(roomCode)
                              .OnceAsync<Room>())?.FirstOrDefault();

            if (room == null)
            {
                Device.BeginInvokeOnMainThread(() => { Navigation.PopAsync(); });
            }
        }

        // Event handler for leave room button
        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            await RemoveUser();
            await Navigation.PopAsync();
        }

        // Remove user from room
        private async Task RemoveUser()
        {
            await App.Database
                     .Child("rooms")
                     .Child(roomCode)
                     .Child("Users")
                     .Child(clientUserId)
                     .DeleteAsync();
        }

        // Event handler for copying room code to clipboard
        private void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(roomCode);
        }
    }
}
