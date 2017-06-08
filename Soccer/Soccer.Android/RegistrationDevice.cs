using Soccer.Interfaces;
using Gcm.Client;
using Xamarin.Forms;
using Android.Util;

[assembly: Dependency(typeof(Soccer.Droid.RegistrationDevice))]

namespace Soccer.Droid
{
    public class RegistrationDevice : IRegisterDevice
    {
        #region Methods
        public void RegisterDevice()
        {
            var mainActivity = MainActivity.GetInstance();
            GcmClient.CheckDevice(mainActivity);
            GcmClient.CheckManifest(mainActivity);

            Log.Info("MainActivity", "Registering...");
            GcmClient.Register(mainActivity, Constants.SenderID);
        }
        #endregion
    }
}