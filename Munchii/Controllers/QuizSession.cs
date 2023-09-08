using System;
using System.Linq;
using Xamarin.Forms;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Database.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Munchii.Services
{
    public class QuizSession : IDisposable // Implement IDisposable to clean up resources
    {
        private readonly string roomCode;
        private Timer keepAliveTimer;
        private readonly object timerLock = new object(); // Object for synchronization

        public QuizSession(string roomCode)
        {
            this.roomCode = roomCode;
            StartKeepAliveUpdates();
        }

        public void StartKeepAliveUpdates()
        {
            lock (timerLock)
            {
                if (keepAliveTimer != null) // Ensure that the timer isn't already running
                    return;

                UpdateKeepAlive();

                keepAliveTimer = new Timer(async _ => await UpdateKeepAlive(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            }
        }

        private async Task UpdateKeepAlive()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    await App.Database
                        .Child("rooms")
                        .Child(roomCode)
                        .Child("KeepAlive")
                        .PutAsync(timestamp);
                    break;
                }
                catch
                {
                    if (--retries == 0) throw; // Re-throw the exception if the last retry fails
                    await Task.Delay(1000); // Wait for 1 second before retrying
                }
            }
        }

        public void StopKeepAliveUpdates()
        {
            lock (timerLock)
            {
                if (keepAliveTimer == null) // Ensure the timer is currently running
                    return;

                keepAliveTimer.Dispose();
                keepAliveTimer = null;
            }
        }

        public void Dispose() // Dispose method to clean up resources
        {
            StopKeepAliveUpdates();
        }
    }
}
