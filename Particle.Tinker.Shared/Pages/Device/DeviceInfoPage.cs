using Particle.SDK;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages.Device
{
    partial class DeviceInfoPage
    {
        #region Private Members

        private ParticleDevice particleDevice = null;
        private Image refreshImage = null;
        private ProgressRing refreshProgressRing = null;

        #endregion

        #region Interaction Methods

        private void FunctionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var startupVariableData = new TinkerPageStartupVariableData();
            startupVariableData.ParticleDevice = particleDevice;
            startupVariableData.SelectedItem = (string)e.AddedItems[0];

            Frame.Navigate(typeof(FunctionsPage), startupVariableData);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (refreshImage == null)
            {
                refreshImage = FlyoutHelper.FindNameInContainer<Image>(RefreshButton, "");
                refreshProgressRing = FlyoutHelper.FindNameInContainer<ProgressRing>(RefreshButton, "");
            }

            refreshImage.Visibility = Visibility.Collapsed;
            refreshProgressRing.IsActive = true;

            await particleDevice.RefreshAsync();

            refreshProgressRing.IsActive = false;
            refreshImage.Visibility = Visibility.Visible;
        }
        
        private void VariablesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var startupVariableData = new TinkerPageStartupVariableData();
            startupVariableData.ParticleDevice = particleDevice;
            startupVariableData.SelectedItem = (KeyValuePair<string,string>)e.AddedItems[0];

            Frame.Navigate(typeof(VariablesPage), startupVariableData);
        }

        #endregion

        #region Private Methods

        private void SetupPage()
        {
            DeviceGrid.DataContext = particleDevice;
        }

        #endregion
    }
}
