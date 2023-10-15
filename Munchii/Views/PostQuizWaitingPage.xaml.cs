using Firebase.Database.Query;
using System;
using Xamarin.Forms;

namespace Munchii
{
    public partial class PostQuizWaitingPage : ContentPage
    {
        private string roomCode;
        private string clientUserId;

        // Keep track of the subscription
        private IDisposable userSubscription;

        public PostQuizWaitingPage(string roomCode, string clientUserId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            this.roomCode = roomCode;
            this.clientUserId = clientUserId;

            try
            {
                ListenForAllQuizzesSubmitted();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up listener: {ex.Message}");
            }
        }

        private bool navigationInProgress = false;

        private bool hasNavigated = false;

        private void NavigateToResultPage()
        {
            if (hasNavigated)
                return;

            hasNavigated = true;
            try
            {
                Navigation.PushAsync(new ResultPage(roomCode)); // Add roomCode here
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to ResultPage: {ex.Message}");
            }
        }

        private void ListenForAllQuizzesSubmitted()
        {
            userSubscription = App.Database
                .Child("rooms")
                .Child(roomCode)
                .Child("Users")
                .AsObservable<User>()
                .Subscribe(firebaseEvent =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            var usersSnapshot = await App.Database
                                .Child("rooms")
                                .Child(roomCode)
                                .Child("Users")
                                .OnceAsync<User>();

                            bool allUsersSubmitted = true;

                            foreach (var userSnapshot in usersSnapshot)
                            {
                                if (!userSnapshot.Object.QuizSubmitted)
                                {
                                    allUsersSubmitted = false;
                                    break;
                                }
                            }

                            if (allUsersSubmitted)
                            {
                                NavigateToResultPage();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error in ListenForAllQuizzesSubmitted: {ex.Message}");
                        }
                    });
                });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe from the observable when the page disappears
            userSubscription?.Dispose();
        }
    }
}
