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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                App.CleanupResources();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        private async void OnHostClicked(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while attempting to host a room. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error while hosting: {ex.Message}");
            }
        }

        private async void OnJoinClicked(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while attempting to join a room. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error while joining: {ex.Message}");
            }
        }

        private bool IsConnectionAvailable()
        {
            try
            {
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    var profiles = Connectivity.ConnectionProfiles;
                    if (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking network access: {ex.Message}");
            }

            return false;
        }
    }
}
