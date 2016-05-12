using Particle.SDK;
using Particle.SDK.Models;
using Particle.Setup;
using Particle.Tinker.Pages.Device;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        Guid? sparkEventListenerID = null;

#if WINDOWS_PHONE_APP
        private EventHandler<Windows.Phone.UI.Input.BackPressedEventArgs> hardwareButtonsBackPressed = null;
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
            AdjustFlyoutMargin(AddFlyout);
            AddFlyout.ShowAt(DeviceListBox);
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
            ShowDeviceMenu(sender);
        }

        private void DeviceListBoxItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            ShowDeviceMenu(sender);
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

        private void AdjustFlyoutMargin(MenuFlyout menuFlyout)
        {
            var lastItem = menuFlyout.Items.Last();
            lastItem.Margin = new Thickness(0, 0, 0, UI.VisibleBoundsWindow.VisibleBounds.NavigationBarHeight.Value);
        }

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
            softAPSettings.OnSoftAPExit += SoftAPSettings_OnSoftAPExit;

#if WINDOWS_PHONE_APP
            hardwareButtonsBackPressed = new EventHandler<Windows.Phone.UI.Input.BackPressedEventArgs>(delegate (object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
            {
                e.Handled = SoftAP.BackButtonPressed();
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
                var sortedDevices = devices.OrderByDescending(device => device.Connected).ThenBy(device => device.Name).ToList();
                TinkerData.Devices = new ObservableCollection<ParticleDevice>(sortedDevices);
                DeviceListBox.DataContext = TinkerData.Devices;

                if (sparkEventListenerID == null)
                    sparkEventListenerID = await ParticleCloud.SharedCloud.SubscribeToDevicesEventsWithPrefixAsync(SparkMessages, "spark");
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

        private void ShowDeviceMenu(object sender)
        {
            var senderElement = sender as FrameworkElement;
            var particleDevice = (ParticleDevice)senderElement.DataContext;

            var deviceFlyout = (MenuFlyout)Resources["DeviceFlyout"];

            var reFlashTinkerMenuButton = (MenuFlyoutItem)FindName("ReFlashTinkerMenuButton");
            reFlashTinkerMenuButton.Visibility = TinkerData.CanFlashTinker(particleDevice) ? Visibility.Visible : Visibility.Collapsed;

            AdjustFlyoutMargin(deviceFlyout);

            deviceFlyout.ShowAt(senderElement);
        }

        private void SoftAPSettings_OnSoftAPExit()
        {
#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= hardwareButtonsBackPressed;
#endif
        }

        private async void SparkMessages(object sender, ParticleEventResponse particeEvent)
        {
            var particleDevice = TinkerData.Devices.FirstOrDefault(device => device.Id == particeEvent.DeviceId);
            if (particleDevice == null)
                return;

            switch (particeEvent.Name)
            {
                case "spark/status":
                    if (particleDevice.IsFlashing && particeEvent.Data.Equals("online"))
                        particleDevice.FlagFlashStatusChange(true);

                    bool success = await particleDevice.RefreshAsync();
                    if (success)
                    {
                        var sortedDevices = TinkerData.Devices.OrderByDescending(device => device.Connected).ThenBy(device => device.Name).ToList();
                        var newIndex = sortedDevices.IndexOf(particleDevice);
                        var oldIndex = TinkerData.Devices.IndexOf(particleDevice);
                        if (newIndex != oldIndex)
                        {
                            TinkerData.Devices.Remove(particleDevice);
                            TinkerData.Devices.Insert(newIndex, particleDevice);
                        }
                    }
                    break;

                case "spark/flash/status":
                    if (particeEvent.Data.StartsWith("started"))
                        particleDevice.FlagFlashStatusChange();

                    break;
            }
        }

        #endregion
    }
}
