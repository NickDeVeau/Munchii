using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Database;
using Munchii.Services;

namespace Munchii
{
    public partial class App : Application
    {
        public QuizSession CurrentQuizSession { get; set; }
        public static FirebaseClient Database { get; set; }
        public App ()
        {
            App.Current.UserAppTheme = OSAppTheme.Light;

            InitializeComponent();

            Database = new FirebaseClient("https://munchii-8986a-default-rtdb.firebaseio.com");

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }

      


        public static void CleanupResources()
        {
            // Dispose of the current QuizSession, if any
            ((App)Application.Current).CurrentQuizSession?.Dispose();
            ((App)Application.Current).CurrentQuizSession = null;

            // Stop the LastSeenUpdater
            LastSeenUpdater.Instance.Stop();

            // ... any other cleanup
        }
    }
}

