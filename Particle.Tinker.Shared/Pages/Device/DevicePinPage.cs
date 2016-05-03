using Particle.SDK;
using Particle.Tinker.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Device
{
    partial class DevicePinPage
    {
        #region Private Members

        private string analogPinWriteCaption;
        private ParticleDevice particleDevice = null;
        private List<Pin> pins;

        #endregion

        #region Interaction Methods

        private void AnalogPinSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int new_value = (int)e.NewValue;
            AnalogPinValueText.Text = new_value.ToString();
        }

        private void AnalogWriteButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            AnalogValueFlyout.Hide();
        }

        private void AnalogWriteButtonSet_Click(object sender, RoutedEventArgs e)
        {
            AnalogValueFlyout.Hide();

            var pin = (Pin)AnalogPinSlider.DataContext;
            pin.AnalogWriteAsync((int)AnalogPinSlider.Value);
        }
                
        private void AskReFlashTinkerButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHelper.AskReFlashTinker(particleDevice, this, MoreButton);
        }

        private void AskRenameDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHelper.AskRenameDevice(particleDevice, this, (FrameworkElement)sender);
        }

        private void AskUnclaimDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHelper.AskUnclaimDevice(particleDevice, this, MoreButton);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void Documentation_Click(object sender, RoutedEventArgs e)
        {
            DocumentationFlyout.ShowAt(MoreButton);
        }

        private void DocumentationParticleApp_Click(object sender, RoutedEventArgs e)
        {
            var success = Launcher.LaunchUriAsync(new Uri("https://docs.particle.io/guide/getting-started/tinker/"));
        }

        private void DocumentationSettingPhoton_Click(object sender, RoutedEventArgs e)
        {
            var success = Launcher.LaunchUriAsync(new Uri("https://docs.particle.io/guide/getting-started/connect/"));
        }

        private void DeviceInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DeviceInfoPage), particleDevice);
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventsPage), particleDevice);
        }

        private void PinButton_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var pinButton = (PinButton)sender;
            var pin = (Pin)pinButton.DataContext;
            pin.ResetPin();
            SavePinState(pin);
        }

        private void PinButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pinButton = (PinButton)sender;
            var pin = (Pin)pinButton.DataContext;

            if (pin.ConfiguredAction != PinAction.None)
                PinUpdate(pinButton);
            else
                SetupButton(pinButton);
        }

        private void PinScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ResizePinGrid();
        }

        private void PinSetupAnalogRead_Click(object sender, RoutedEventArgs e)
        {
            var pinButton = GetPinButtonFromControl(sender);
            if (pinButton == null)
                return;

            SetConfiguredAction(pinButton, PinAction.AnalogRead);
        }

        private void PinSetupAnalogWrite_Click(object sender, RoutedEventArgs e)
        {
            var pinButton = GetPinButtonFromControl(sender);
            if (pinButton == null)
                return;

            SetConfiguredAction(pinButton, PinAction.AnalogWrite);
        }

        private void PinSetupAnalogWriteDac_Click(object sender, RoutedEventArgs e)
        {
            var pinButton = GetPinButtonFromControl(sender);
            if (pinButton == null)
                return;

            SetConfiguredAction(pinButton, PinAction.AnalogWriteDac);
        }

        private void PinSetupDigitalgRead_Click(object sender, RoutedEventArgs e)
        {
            var pinButton = GetPinButtonFromControl(sender);
            if (pinButton == null)
                return;

            SetConfiguredAction(pinButton, PinAction.DigitalRead);
        }

        private void PinSetupDigitalWrite_Click(object sender, RoutedEventArgs e)
        {
            var pinButton = GetPinButtonFromControl(sender);
            if (pinButton == null)
                return;

            SetConfiguredAction(pinButton, PinAction.DigitalWrite);
        }

        private void ResetAllPins_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pin in pins)
            {
                pin.ResetPin();
                SavePinState(pin);
            }
        }

        private void RootGrid_LayoutUpdated(object sender, object e)
        {
            ResizePinGrid();
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            SupportFlyout.ShowAt(MoreButton);
        }

        private void SupportParticleCommunity_Click(object sender, RoutedEventArgs e)
        {
            var success = Launcher.LaunchUriAsync(new Uri("http://community.particle.io"));
        }

        private void SupportParticleSupport_Click(object sender, RoutedEventArgs e)
        {
            var success = Launcher.LaunchUriAsync(new Uri("https://docs.particle.io/support"));
        }

        private void WelcomToTinkerGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            WelcomToTinkerGrid.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Private Methods

        private PinButton GetPinButtonFromControl(object sender)
        {
            var control = (Control)sender;
            if (control != null)
            {
                var pinButton = (PinButton)control.DataContext;
                if (pinButton != null)
                {
                    return pinButton;
                }
            }

            return null;
        }

        private void LoadPins()
        {
            if (!TinkerData.HasShownWelcomeToTinker)
            {
                WelcomToTinkerGrid.Visibility = Visibility.Visible;
                ApplicationData.Current.LocalSettings.Values["WelcomeToTinker"] = true;
            }

            var devicePinActions = TinkerData.GetDevicePinActions(particleDevice.Id);

            ResourceLoader resourceLoader = new ResourceLoader();
            analogPinWriteCaption = resourceLoader.GetString("AnalogPinWriteCaption");

            pins = Pin.GetDevicePins(particleDevice);

            foreach (var pin in pins)
            {
                if (devicePinActions.ContainsKey(pin.TinkerId))
                    pin.ConfiguredAction = devicePinActions[pin.TinkerId];

                var pinButton = (PinButton)FindName(pin.TinkerId);
                pinButton.DataContext = pin;
                pinButton.Tapped += PinButton_Tapped;
                pinButton.Holding += PinButton_Holding;

                if (pin.PinType == PinType.B || pin.PinType == PinType.C)
                    LowerPinGrid.Visibility = Visibility.Visible;
            }
        }

        private async void PinUpdate(PinButton pinButton)
        {
            var pin = (Pin)pinButton.DataContext;

            switch (pin.ConfiguredAction)
            {
                case PinAction.AnalogRead:
                    pin.AnalogReadAsync();
                    break;
                case PinAction.AnalogWrite:
                case PinAction.AnalogWriteDac:
                    AnalogPinWriteCaption.Text = string.Format(analogPinWriteCaption, pin.Caption);
                    AnalogPinSlider.Minimum = 0;
                    AnalogPinSlider.Maximum = pin.MaxAnalogWriteValue;
                    AnalogPinSlider.Value = pin.Value;
                    AnalogPinSlider.DataContext = pin;
                    AnalogPinValueText.Text = pin.Value.ToString();
                    AnalogValueFlyout.ShowAt(pinButton);
                    break;
                case PinAction.DigitalRead:
                    await pin.DigitalReadAsync();
                    break;
                case PinAction.DigitalWrite:
                    pin.DigitalWriteAsync();
                    break;
            }
        }

        private void ResizePinGrid()
        {
            if (UpperPinGrid.Height == PinScrollViewer.ActualHeight)
                return;

            UpperPinGrid.Height = PinScrollViewer.ActualHeight;
            LowerPinGrid.Height = PinScrollViewer.ActualHeight / 8 * 6;
        }

        private void SavePinState(Pin pin)
        {
            TinkerData.SetDevicePinAction(particleDevice.Id, pin.TinkerId, pin.ConfiguredAction);
        }
        
        private void SetConfiguredAction(PinButton pinButton, PinAction pinAction = PinAction.None)
        {
            PinSetup.Hide();
            var pin = (Pin)pinButton.DataContext;
            pin.Value = 0;
            pin.ConfiguredAction = pinAction;
            SavePinState(pin);
        }

        private void SetupButton(PinButton pinButton)
        {
            var pin = (Pin)pinButton.DataContext;

            PinSetupCaption.Text = pin.Caption;
            PinSetupAnalogRead.Visibility = pin.Functions.Contains(PinAction.AnalogRead) ? Visibility.Visible : Visibility.Collapsed;
            PinSetupAnalogWrite.Visibility = pin.Functions.Contains(PinAction.AnalogWrite) ? Visibility.Visible : Visibility.Collapsed;
            PinSetupAnalogWriteDac.Visibility = pin.Functions.Contains(PinAction.AnalogWriteDac) ? Visibility.Visible : Visibility.Collapsed;
            PinSetupDigitalgRead.Visibility = pin.Functions.Contains(PinAction.DigitalRead) ? Visibility.Visible : Visibility.Collapsed;
            PinSetupDigitalWrite.Visibility = pin.Functions.Contains(PinAction.DigitalWrite) ? Visibility.Visible : Visibility.Collapsed;

            PinSetupAnalogRead.DataContext = pinButton;
            PinSetupAnalogWrite.DataContext = pinButton;
            PinSetupAnalogWriteDac.DataContext = pinButton;
            PinSetupDigitalgRead.DataContext = pinButton;
            PinSetupDigitalWrite.DataContext = pinButton;

            PinSetup.ShowAt(pinButton);
        }

        #endregion
    }
}
