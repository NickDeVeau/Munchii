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
    public partial class HostPage : ContentPage
    {
        private string roomCode;
        private string HostUserId;

        private Func<bool> timerAction;

        public HostPage()
        {
            InitializeComponent();

            



            NavigationPage.SetHasNavigationBar(this, false);
            SetupRoom();

            Digit1.Text = roomCode[0].ToString();
            Digit2.Text = roomCode[1].ToString();
            Digit3.Text = roomCode[2].ToString();
            Digit4.Text = roomCode[3].ToString();

            timerAction = TimerTick;

            Device.StartTimer(TimeSpan.FromSeconds(1), timerAction);
            ((App)Application.Current).CurrentQuizSession = new QuizSession(roomCode);

        }
        private bool TimerTick()
        {
            Task.Run(() => CheckNullRoom());

            return true;
        }

        private async void SetupRoom()
        {
            var random = new Random();
            roomCode = new string(Enumerable.Range(0, 4).Select(_ => (char)('0' + random.Next(0, 10))).ToArray());
            RoomCodeLabel.Text = $"Room Code: {roomCode}";

            HostUserId = Guid.NewGuid().ToString();
            var hostUser = new User
            {
                Id = HostUserId,
                Username = "Host",
                QuizSubmitted = false
            };

            var initialUsers = new Dictionary<string, User>
    {
        { HostUserId, hostUser }
    };

            var room = new Room
            {
                RoomCode = roomCode,
                Users = initialUsers,
                HostId = HostUserId,
            };

            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .PutAsync(room);

            // Start the LastSeenUpdater
            LastSeenUpdater.Instance.Start(roomCode, HostUserId);

            StartUserCountUpdateTimer();
        }

        private IDisposable userSubscription;

        private void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(roomCode);
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

        private void StartUserCountUpdateTimer()
        {
            var userList = new List<User>();
            userSubscription = App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .AsObservable<User>()
                .Subscribe(firebaseUser =>
                {
                    // Firebase user event types are: Added, Changed, Removed
                    // If a user is added or changed, update or add them to the list
                    if (firebaseUser.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        var existingUser = userList.FirstOrDefault(x => x.Id == firebaseUser.Key);
                        if (existingUser != null)
                        {
                            userList.Remove(existingUser);
                        }
                        userList.Add(firebaseUser.Object);
                    }
                    // If a user is removed, remove them from the list
                    else if (firebaseUser.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                    {
                        var existingUser = userList.FirstOrDefault(x => x.Id == firebaseUser.Key);
                        if (existingUser != null)
                        {
                            userList.Remove(existingUser);
                        }
                    }

                    // Update the user count label with the current user count
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserCountLabel.Text = $"{userList.Count} participants";
                    });
                }, ex => Console.WriteLine($"Subscription Error: {ex.Message}"));
        }

        private async void OnStartQuizClicked(object sender, EventArgs e)
        {
            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("QuizStarted")
                .PutAsync(true);
            await Navigation.PushAsync(new DealBreakerPage(roomCode, HostUserId));
        }

        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            await RemoveUser(HostUserId);
            // NOTE: If the host is the last user in the room, and no new user is added within the next 2 minutes,
            // the room will be considered stale and deleted by the Firebase function.
            await Navigation.PopAsync();
        }

        private async Task RemoveUser(string userId)
        {
            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .Child(userId)
                .DeleteAsync();
        }

       
    }
}
