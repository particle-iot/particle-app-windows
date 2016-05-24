using Newtonsoft.Json;
using Particle.SDK;
using Particle.Setup;
using Particle.Tinker.Pages;
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
        private static ObservableCollection<ParticleDevice> devices = null;
        private static HashSet<string> deviceNames = null;
        private static Dictionary<string, Dictionary<string, PinAction>> devicesPinActions = null;
        private static SetupConfig setupConfig = null;

        #endregion

        #region Properties

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

        public static SetupConfig SetupConfig
        {
            get
            {
                return setupConfig;
            }
        }

        #endregion

        #region Public Methods

        public static bool CanFlashTinker(ParticleDevice particleDevice)
        {
            switch (particleDevice.KnownProductId)
            {
                case ParticleDeviceType.ParticleCore:
                case ParticleDeviceType.ParticlePhoton:
                case ParticleDeviceType.ParticleP1:
                case ParticleDeviceType.ParticleElectron:
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

        public static void InitSetup(Frame frame)
        {
            setupConfig = new SetupConfig();
            setupConfig.AppFrame = frame;
            setupConfig.CompletionPageType = typeof(DevicesPage);
            setupConfig.CurrentDeviceNames = GetDeviceNames();
            setupConfig.OnSetupLogout += Logout;
        }

        public static void Logout()
        {
            devices = null;
            deviceNames = null;
            devicesPinActions = null;

            RemoveLocalSetting("DevicePinActions");
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
