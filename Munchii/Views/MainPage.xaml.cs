using Munchii;
using Munchii.Services;
using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Linq;

namespace Munchii
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnHostClicked(object sender, EventArgs e)
        {
            if (IsConnectionAvailable())
            {
                await Navigation.PushAsync(new HostPage());
            }
            else
            {
                await DisplayAlert("Error", "Poor or no Wi-Fi connection detected. Please check your connection.", "OK");
            }
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            if (IsConnectionAvailable())
            {
                await Navigation.PushAsync(new JoinPage());
            }
            else
            {
                await DisplayAlert("Error", "Poor or no Wi-Fi connection detected. Please check your connection.", "OK");
            }
        }

        // Checks if there is an active Wi-Fi connection
        private bool IsConnectionAvailable()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                // Further check if connection is Wi-Fi (this step is optional)
                var profiles = Connectivity.ConnectionProfiles;
                if (profiles.Contains(ConnectionProfile.WiFi))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
