﻿using Particle.SDK;
using Particle.Setup;
using Particle.Setup.Models;
using Particle.Tinker.Pages.Device;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages
{
    partial class DevicesPage
    {
        #region Private Members

 #if WINDOWS_PHONE_APP
        private EventHandler<Windows.Phone.UI.Input.BackPressedEventArgs> hardwareButtonsBackPressed = null;
 #else
        private object hardwareButtonsBackPressed;
 #endif

        #endregion

        #region Event Handlers

        private void ParticleCloud_ClientUnauthorized()
        {
            TinkerData.Logout();
        }

        #endregion

        #region Interaction Methods

        private void AboutAppBarButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AddFlyout.ShowAt(DevicesCommandBar);
        }

        private void AddElectronButton_Click(object sender, RoutedEventArgs e)
        {
            var success = Launcher.LaunchUriAsync(new Uri("https://setup.particle.io"));
        }

        private void AddPhotonButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchSoftAP();
        }

        private void AskReFlashTinkerButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            var listViewItem = (ListViewItem)DeviceListBox.ContainerFromItem(particleDevice);
            FlyoutHelper.AskReFlashTinker(particleDevice, this, listViewItem);
        }

        private void AskRenameDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            var listViewItem = (ListViewItem)DeviceListBox.ContainerFromItem(particleDevice);
            FlyoutHelper.AskRenameDevice(particleDevice, this, listViewItem);
        }

        private void AskUnclaimDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            var listViewItem = (ListViewItem)DeviceListBox.ContainerFromItem(particleDevice);
            FlyoutHelper.AskUnclaimDevice(particleDevice, this, listViewItem);
        }

        private void DeviceListBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var particleDevice = (ParticleDevice)e.ClickedItem;
            if (particleDevice == null)
                return;

            switch (particleDevice.State)
            {
                case ParticleDeviceState.Unknown:
                case ParticleDeviceState.Offline:
                    DeviceOfflineFlyout.ShowAt(DeviceListBox);
                    break;
                case ParticleDeviceState.Flashing:
                    break;
                case ParticleDeviceState.Online:
                    DeviceNotTinkerReFlashButton.DataContext = particleDevice;
                    DeviceNotTinkerTinkerButton.DataContext = particleDevice;
                    DeviecNotTinketFlyout.ShowAt(DeviceListBox);
                    break;
                case ParticleDeviceState.Tinker:
                    Frame.Navigate(typeof(DevicePinPage), particleDevice);
                    break;
            }
        }

        private void DeviceListBoxItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var particleDevice = (ParticleDevice)senderElement.DataContext;

            var deviceFlyout = (MenuFlyout)Resources["DeviceFlyout"];
            deviceFlyout.ShowAt(senderElement);
        }

        private void DeviceOfflineOkButton_Click(object sender, RoutedEventArgs e)
        {
            DeviceOfflineFlyout.Hide();
        }

        private void DeviceNotTinkerCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DeviecNotTinketFlyout.Hide();
        }

        private void DeviceNotTinkerTinkerButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            Frame.Navigate(typeof(DevicePinPage), particleDevice);
        }

        private void DeviceInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            Frame.Navigate(typeof(DeviceInfoPage), particleDevice);
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            var particleDevice = GetParticleDeviceFromControl(sender);
            if (particleDevice == null)
                return;

            Frame.Navigate(typeof(EventsPage), particleDevice);
        }

        private void LogoutAppBarButton_click(object sender, RoutedEventArgs e)
        {
            TinkerData.Logout();
        }

        private async void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadDevicesAsync();
        }

        #endregion

        #region Private Methods

        private ParticleDevice GetParticleDeviceFromControl(object sender)
        {
            var control = (Control)sender;
            if (control != null)
            {
                var particleDevice = (ParticleDevice)control.DataContext;
                if (particleDevice != null)
                {
                    return particleDevice;
                }
            }

            return null;
        }

        private void LaunchSoftAP()
        {
            UI.WindowsRuntimeResourceManager.InjectIntoResxGeneratedApplicationResourcesClass(typeof(Particle.Setup.SetupResources));

            SoftAPSettings softAPSettings = new SoftAPSettings();
            softAPSettings.AppFrame = Frame;
            softAPSettings.CompletionPageType = GetType();
            softAPSettings.Username = TinkerData.Username;
            softAPSettings.CurrentDeviceNames = TinkerData.GetDeviceNames();

#if WINDOWS_PHONE_APP
            hardwareButtonsBackPressed = new EventHandler<Windows.Phone.UI.Input.BackPressedEventArgs>(delegate (object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
            {
                if (SoftAP.SoftAPResult.Result != SoftAPSetupResult.Started)
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= hardwareButtonsBackPressed;
                    if (SoftAP.SoftAPResult.Result != SoftAPSetupResult.NotStarted)
                        return;
                }

                if (Frame.CanGoBack)
                {
                    e.Handled = true;
                    Frame.GoBack();
                }
            });

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += hardwareButtonsBackPressed;
#endif

            SoftAP.Start(softAPSettings);
        }

        private async Task LoadDevicesAsync()
        {
            LoadDevicesProgress.IsActive = true;
            DeviceListBox.DataContext = null;

            try
            {
                var devices = await ParticleCloud.SharedCloud.GetDevicesAsync();
                TinkerData.Devices = new ObservableCollection<ParticleDevice>(devices);
                DeviceListBox.DataContext = TinkerData.Devices;
            }
            catch
            {
            }
            finally
            {
                LoadDevicesProgress.IsActive = false;
            }
        }
        
        private async Task SetupTinkerAsync()
        {
            ParticleCloud.SharedCloud.ClientUnauthorized += ParticleCloud_ClientUnauthorized;
            await LoadDevicesAsync();
        }

        #endregion
    }
}
