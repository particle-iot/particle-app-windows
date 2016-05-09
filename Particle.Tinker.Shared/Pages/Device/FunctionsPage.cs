using Particle.SDK;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages.Device
{
    partial class FunctionsPage
    {
        #region Private Members

        private TinkerPageStartupVariableData startupVariableData = null;
        private ParticleDevice particleDevice = null;

        #endregion

        #region Interaction Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private async void CallButton_Click(object sender, RoutedEventArgs e)
        {
            SetEnableState(false);
            ErrorBorder.Visibility = Visibility.Collapsed;

            ResultTextBox.Text = "";

            var function = (string)FunctionComboBox.SelectedItem;
            var arg = ArgumentsTextBox.Text;

            try
            {
                var functionValue = await particleDevice.RunFunctionAsync(function, arg);
                if (functionValue != null)
                    ResultTextBox.Text = Convert.ToString(functionValue.ReturnValue);
            }
            catch
            {
                ResourceLoader resourceLoader = new ResourceLoader();
                ErrorText.Text = resourceLoader.GetString("Error");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }
        
        private void FunctionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResultTextBox.Text = "";
        }

        #endregion

        #region Private Methods

        private void SetEnableState(bool enabled)
        {
            Progress.IsIndeterminate = !enabled;

            CallButton.IsEnabled = enabled;

            FunctionComboBox.IsEnabled = enabled;
            ArgumentsTextBox.IsEnabled = enabled;
        }

        private void SetupPage()
        {
            particleDevice = startupVariableData.ParticleDevice;

            DataGrid.DataContext = particleDevice;

            var selectedItem = (string)startupVariableData.SelectedItem;

            for (var index = 0; index < FunctionComboBox.Items.Count; ++index)
            {
                var item = (string)FunctionComboBox.Items[index];
                if (item == selectedItem)
                {
                    FunctionComboBox.SelectedIndex = index;
                    break;
                }
            }
        }

        #endregion
    }
}
