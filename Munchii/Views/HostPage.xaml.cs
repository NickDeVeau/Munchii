using System;
using System.Linq;
using Xamarin.Forms;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Database.Query;
using System.Threading.Tasks;
using Munchii.Services;
using Xamarin.Essentials;
using System;
using Firebase.Database.Streaming;

namespace Munchii
{
    public partial class HostPage : ContentPage
    {
        private string roomCode;
        private string hostUserId;
        private Func<bool> timerAction;
        private IDisposable userSubscription;

        public HostPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            SetupRoom();
            DisplayRoomCode();

            timerAction = TimerTick;
            Device.StartTimer(TimeSpan.FromSeconds(1), timerAction);
            ((App)Application.Current).CurrentQuizSession = new QuizSession(roomCode);
        }

        private async void SetupRoom()
        {
            GenerateRoomCode();
            await InitializeFirebaseRoom();
        }

        private void GenerateRoomCode()
        {
            var random = new Random();
            roomCode = new string(Enumerable.Range(0, 4).Select(_ => (char)('0' + random.Next(0, 10))).ToArray());
            RoomCodeLabel.Text = $"Room Code: {roomCode}";
        }

        private async Task InitializeFirebaseRoom()
        {
            hostUserId = Guid.NewGuid().ToString();
            var hostUser = new User
            {
                Id = hostUserId,
                Username = "Host",
                QuizSubmitted = false
            };

            var initialUsers = new Dictionary<string, User>
            {
                { hostUserId, hostUser }
            };

            var room = new Room
            {
                RoomCode = roomCode,
                Users = initialUsers,
                HostId = hostUserId,
            };

            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .PutAsync(room);

            LastSeenUpdater.Instance.Start(roomCode, hostUserId);
            StartUserCountUpdateTimer();
        }

        private void DisplayRoomCode()
        {
            Digit1.Text = roomCode[0].ToString();
            Digit2.Text = roomCode[1].ToString();
            Digit3.Text = roomCode[2].ToString();
            Digit4.Text = roomCode[3].ToString();
        }

        private bool TimerTick()
        {
            Task.Run(() => CheckNullRoom());
            return true;
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
                .Subscribe(firebaseEvent =>
                {
                    HandleFirebaseUserEvents(firebaseEvent, userList);
                }, ex => Console.WriteLine($"Subscription Error: {ex.Message}"));
        }

        private void HandleFirebaseUserEvents(FirebaseEvent<User> firebaseEvent, List<User> userList)
        {
            if (firebaseEvent.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
            {
                UpdateOrAddUserToList(firebaseEvent, userList);
            }
            else if (firebaseEvent.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
            {
                RemoveUserFromList(firebaseEvent, userList);
            }
            UpdateUserCountLabel(userList.Count);
        }

        private void UpdateOrAddUserToList(FirebaseEvent<User> firebaseEvent, List<User> userList)
        {
            var existingUser = userList.FirstOrDefault(x => x.Id == firebaseEvent.Key);
            if (existingUser != null)
            {
                userList.Remove(existingUser);
            }
            userList.Add(firebaseEvent.Object);
        }

        private void RemoveUserFromList(FirebaseEvent<User> firebaseEvent, List<User> userList)
        {
            var existingUser = userList.FirstOrDefault(x => x.Id == firebaseEvent.Key);
            if (existingUser != null)
            {
                userList.Remove(existingUser);
            }
        }

        private void UpdateUserCountLabel(int count)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                UserCountLabel.Text = $"{count} participants";
            });
        }

        private async void OnStartQuizClicked(object sender, EventArgs e)
        {
            await App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("QuizStarted")
                .PutAsync(true);
            await Navigation.PushAsync(new DealBreakerPage(roomCode, hostUserId));
        }

        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            await RemoveUser(hostUserId);
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

        private void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(roomCode);
        }
    }
}
