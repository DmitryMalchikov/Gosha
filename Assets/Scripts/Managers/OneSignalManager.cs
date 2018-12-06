using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class OneSignalManager : MonoBehaviour
    {

        void Start()
        {
            // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
            // OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);

            OneSignal.StartInit("a679caa4-ff0e-4bc9-a39b-57fa977e8e1a")
                .HandleNotificationOpened(HandleNotificationOpened)
                .EndInit();

            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.None;

            // Call syncHashedEmail anywhere in your app if you have the user's email.
            // This improves the effectiveness of OneSignal's "best-time" notification scheduling feature.
            // OneSignal.syncHashedEmail(userEmail);
        }

        // Gets called when the player opens the notification.
        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
        }
    }
}
