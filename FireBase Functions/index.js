const functions = require("firebase-functions");
const admin = require("firebase-admin");

admin.initializeApp();

exports.checkRoomAlive = functions.pubsub
    .schedule("every 2 minutes")
    .onRun(async (context) => {
      const now = Date.now();
      const roomsSnapshot = await admin.database().ref("rooms").once("value");

      const roomPromises = [];

      roomsSnapshot.forEach((roomSnapshot) => {
        const keepAlive = Number(roomSnapshot.child("KeepAlive").val());

        if (now - keepAlive > 2 * 60 * 1000) {
          roomPromises.push(roomSnapshot.ref.remove());
        } else {
          const usersSnapshot = roomSnapshot.child("Users").val();

          const userPromises = [];

          // Add if statement to filter unwanted properties
          for (const userId in usersSnapshot) {
            if (Object.prototype.hasOwnProperty.call(usersSnapshot, userId)) {
              const lastSeen = Number(usersSnapshot[userId].lastSeen);

              if (now - lastSeen > 1 * 60 * 1000) {
                // Split the line into multiple lines to adhere to max-len rule
                userPromises.push(
                    admin
                        .database()
                        .ref(`rooms/${roomSnapshot.key}/Users/${userId}`)
                        .remove(),
                );
              }
            }
          }

          roomPromises.push(Promise.all(userPromises));
        }
      });

      await Promise.all(roomPromises);

      return null;
    });


exports.incrementSubmittedCount = functions.database
    .ref("/rooms/{roomId}/Users/{userId}")
    .onWrite(async (change, context) => {
      // Only edit data when it is first created.
      if (change.before.exists() || !change.after.exists()) {
        return null;
      }
      // Grab the current value of what was written to the Realtime Database.
      const original = change.after.val();

      // Check if quiz was submitted
      if (original.quizSubmitted === true) {
        const roomId = context.params.roomId;

        const roomRef = admin.database().ref(`/rooms/${roomId}`);

        // Increment the
        // submittedQuizCount
        await roomRef
            .child("submittedQuizCount").transaction((currentCount) => {
              return (currentCount || 0) + 1;
            });
      }

      return null;
    });
