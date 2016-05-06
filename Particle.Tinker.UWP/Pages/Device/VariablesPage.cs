using Particle.SDK;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages.Device
{
    partial class VariablesPage
    {
        #region Private Members

        private TinkerPageStartupVariableData startupVariableData = null;
        private ParticleDevice particleDevice = null;

        #endregion

        #region Interaction Methods

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SetEnableState(false);

            ResultTextBox.Text = "";

            var variable = (KeyValuePair<string,string>)VariablesComboBox.SelectedItem;

            try
            {
                var variableValue = await particleDevice.GetVariableAsync(variable.Key);
                if (variableValue != null)
                    ResultTextBox.Text = Convert.ToString(variableValue.Result);
            }
            catch
            {
                ResourceLoader resourceLoader = new ResourceLoader();
                ErrorText.Text = resourceLoader.GetString("Error");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }

        private void VariablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshButton_Click(sender, null);
        }

        #endregion

        #region Private Methods

        private void SetEnableState(bool enabled)
        {
            Progress.IsIndeterminate = !enabled;

            RefreshButton.IsEnabled = enabled;

            VariablesComboBox.IsEnabled = enabled;
        }

        private void SetupPage()
        {
            particleDevice = startupVariableData.ParticleDevice;

            DataGrid.DataContext = particleDevice;

            for (var index = 0; index < VariablesComboBox.Items.Count; ++index)
            {
                var selectedItem = (KeyValuePair<string, string>)startupVariableData.SelectedItem;

                var item = (KeyValuePair<string, string>)VariablesComboBox.Items[index];
                if (item.Key == selectedItem.Key)
                {
                    VariablesComboBox.SelectedIndex = index;
                    break;
                }
            }
        }

        #endregion
    }
}
