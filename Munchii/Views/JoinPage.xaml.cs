using System;
using Xamarin.Forms;
using Firebase.Database;
using Firebase.Database.Query;

namespace Munchii
{
    public partial class JoinPage : ContentPage
    {
        private string roomCode;
        private string clientUserId;

        public JoinPage()
        {
            InitializeComponent();
            var behavior1 = Digit1.Behaviors[0] as EntryLineBehavior;
            var behavior2 = Digit2.Behaviors[0] as EntryLineBehavior;
            var behavior3 = Digit3.Behaviors[0] as EntryLineBehavior;
            var behavior4 = Digit4.Behaviors[0] as EntryLineBehavior;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnJoinRoomClicked(object sender, EventArgs e)
        {
            roomCode = Digit1.Text + Digit2.Text + Digit3.Text + Digit4.Text;

            if (string.IsNullOrEmpty(roomCode))
            {
                await DisplayAlert("Error", "Please enter a room code.", "OK");
                return;
            }

            Room room = null;

            try
            {
                room = await App.Database
                    .Child("rooms")
                    .Child(roomCode)
                    .OnceSingleAsync<Room>();
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Could not fetch room information. Please try again.", "OK");
                return;
            }

            if (room != null)
            {
                clientUserId = Guid.NewGuid().ToString();
                var username = "User_" + clientUserId;

                var clientUser = new User
                {
                    Id = clientUserId,
                    Username = username,
                    QuizSubmitted = false
                };

                try
                {
                    await App.Database
                        .Child("rooms")
                        .Child(roomCode)
                        .Child("Users")
                        .Child(clientUserId)
                        .PutAsync(clientUser);

                    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    await App.Database
                        .Child("rooms")
                        .Child(roomCode)
                        .Child("KeepAlive")
                        .PutAsync(timestamp);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error adding user: {ex.Message}");
                }

                await Navigation.PushAsync(new ClientLobbyPage(roomCode, clientUserId));
            }
            else
            {
                await DisplayAlert("Error", "Invalid room code.", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            RemoveUserFromRoom();
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            RemoveUserFromRoom();
            return base.OnBackButtonPressed();
        }

        private async void RemoveUserFromRoom()
        {
            try
            {
                await App.Database
                    .Child("rooms")
                    .Child(roomCode)
                    .Child("Users")
                    .Child(clientUserId)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async void OnLeaveRoomClicked(object sender, EventArgs e)
        {
            RemoveUserFromRoom();
            await Navigation.PopAsync();
        }
    }
}
