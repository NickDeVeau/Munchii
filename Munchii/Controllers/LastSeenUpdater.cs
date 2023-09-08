using System;
using Xamarin.Forms;
using Firebase.Database;
using System.Threading;
using System.Threading.Tasks;
using Munchii.Services;
using Firebase.Database.Query;

namespace Munchii.Services
{
    public class LastSeenUpdater
    {
        private const int UpdateInterval = 60000;  // 60 seconds
        private string userId;
        private string roomCode;
        private Timer updateTimer;

        private static LastSeenUpdater instance;
        public static LastSeenUpdater Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LastSeenUpdater();
                }
                return instance;
            }
        }

        private LastSeenUpdater() { }

        public void Start(string roomCode, string userId)
        {
            // If the timer is already running, stop it first.
            Stop();

            this.userId = userId;
            this.roomCode = roomCode;
            updateTimer = new Timer(UpdateLastSeen, null, 0, UpdateInterval);

            System.Diagnostics.Debug.WriteLine($"Started LastSeenUpdater for {userId} in room {roomCode}");
        }

        public void Stop()
        {
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
                System.Diagnostics.Debug.WriteLine($"Stopped LastSeenUpdater for {userId} in room {roomCode}");
            }
        }

        private async void UpdateLastSeen(object state)
        {
            try
            {
                // Get the current Unix timestamp
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Update the user's lastSeen value in the database
                await App.Database
                    .Child("rooms")
                    .Child(roomCode)
                    .Child("Users")
                    .Child(userId)
                    .Child("lastSeen")
                    .PutAsync(timestamp);

                System.Diagnostics.Debug.WriteLine($"Updated LastSeen for {userId} in room {roomCode} to {timestamp}");
            }
            catch (Exception ex)
            {
                // Log any errors
                System.Diagnostics.Debug.WriteLine($"Error updating LastSeen for {userId} in room {roomCode}: {ex.Message}");
            }
        }
    }
}
