using Newtonsoft.Json;
using Particle.SDK;
using Particle.Tinker.Pages.Auth;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker
{
    public class TinkerPageStartupVariableData
    {
        public ParticleDevice ParticleDevice { get; set; }
        public object SelectedItem { get; set; }
    }

    public static class TinkerData
    {
        #region Private Members

        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static Frame applcationFrame = null;
        private static ObservableCollection<ParticleDevice> devices = null;
        private static HashSet<string> deviceNames = null;
        private static Dictionary<string, Dictionary<string, PinAction>> devicesPinActions = null;
        
        #endregion

        #region Properties

        public static string AccessToken
        {
            get
            {
                if (localSettings.Values.ContainsKey("AccessToken"))
                    return (string)localSettings.Values["AccessToken"];
                else
                    return null;
            }
        }

        public static ObservableCollection<ParticleDevice> Devices
        {
            get
            {
                return devices;
            }
            set
            {
                devices = value;
                deviceNames = null;
            }
        }

        public static bool HasShownWelcomeToTinker
        {
            get
            {
                return localSettings.Values.ContainsKey("WelcomeToTinker");
            }
        }

        public static bool HasSignedIn
        {
            get
            {
                return localSettings.Values.ContainsKey("HasSignedIn");
            }
        }

        public static string Username
        {
            get
            {
                if (localSettings.Values.ContainsKey("Username"))
                    return (string)localSettings.Values["Username"];
                else
                    return "";
            }
        }

        #endregion

        #region Public Methods

        public static bool CanFlashTinker(ParticleDevice particleDevice)
        {
            switch (particleDevice.ProductId)
            {
                case ParticleDeviceType.Core:
                case ParticleDeviceType.Photon:
                case ParticleDeviceType.P1:
                case ParticleDeviceType.Electron:
                    return true;
                default:
                    return false;
            }
        }

        public static HashSet<string> GetDeviceNames()
        {
            if (deviceNames == null)
            {
                deviceNames = new HashSet<string>();
                foreach (var device in devices)
                    deviceNames.Add(device.Name);
            }

            return deviceNames;
        }

        public static Dictionary<string, PinAction> GetDevicePinActions(string deviceId)
        {
            SetDevicesPinActions();

            if (devicesPinActions.ContainsKey(deviceId))
                return devicesPinActions[deviceId];
            else
            {
                var devicePinActions = new Dictionary<string, PinAction>();
                devicesPinActions.Add(deviceId, devicePinActions);
                return devicePinActions;
            }
        }

        public static void Login()
        {
            localSettings.Values["HasSignedIn"] = true;
            localSettings.Values["AccessToken"] = ParticleCloud.SharedCloud.AccessToken;
            localSettings.Values["Username"] = ParticleCloud.SharedCloud.Username;
        }

        public static void Logout()
        {
            devices = null;
            deviceNames = null;
            devicesPinActions = null;
            ParticleCloud.SharedCloud.Logout();

            RemoveLocalSetting("AccessToken");
            RemoveLocalSetting("Username");
            RemoveLocalSetting("DevicePinActions");

            applcationFrame.Navigate(typeof(LoginPage));
        }

        public static void SetApplcationFrame(Frame frame)
        {
            applcationFrame = frame;
        }

        public static void SetDevicePinAction(string deviceId, string tinkerId, PinAction pinAction)
        {
            var devicePinActions = GetDevicePinActions(deviceId);

            if (devicePinActions.ContainsKey(tinkerId))
            {
                if (pinAction == PinAction.None)
                {
                    devicePinActions.Remove(tinkerId);
                }
                else
                {
                    devicePinActions[tinkerId] = pinAction;
                }

            }
            else if (pinAction != PinAction.None)
            {
                devicePinActions.Add(tinkerId, pinAction);
            }

            localSettings.Values["DevicePinActions"] = JsonConvert.SerializeObject(devicesPinActions);
        }

        #endregion

        #region Private Methods

        private static void RemoveLocalSetting(string key)
        {
            if (localSettings.Values.ContainsKey(key))
                localSettings.Values.Remove(key);
        }

        private static void SetDevicesPinActions()
        {
            if (devicesPinActions == null)
            {
                if (localSettings.Values.ContainsKey("DevicePinActions"))
                {
                    var devicePinActionsJson = (string)localSettings.Values["DevicePinActions"];
                    devicesPinActions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, PinAction>>>(devicePinActionsJson);
                }
                else
                {
                    devicesPinActions = new Dictionary<string, Dictionary<string, PinAction>>();
                }
            }
        }

        #endregion
    }
}
