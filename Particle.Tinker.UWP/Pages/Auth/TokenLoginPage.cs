using Particle.SDK;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Auth
{
    partial class TokenLoginPage
    {
        #region Interaction Methods

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            TokenError.Visibility = Visibility.Collapsed;
            ErrorBorder.Visibility = Visibility.Collapsed;

            string errorText = "";
            var token = Token.Text;

            if (string.IsNullOrWhiteSpace(token))
            {
                errorText += resourceLoader.GetString("ErrorTokenIsRequired");
                TokenError.Visibility = Visibility.Visible;
            }

            if (errorText.Length != 0)
            {
                ErrorText.Text = errorText;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            SetEnableState(false);

            var success = await ParticleCloud.SharedCloud.TokenLoginAsync(token);

            if (success)
            {
                TinkerData.Login();
                Frame.Navigate(typeof(DevicesPage));
            }
            else
            {
                ErrorText.Text = resourceLoader.GetString("AuthCredentialsError");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }

        private void NoTokenButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                switch (((Control)sender).Name)
                {
                    case "Token":
                        LoginButton.Focus(FocusState.Pointer);
                        LoginButton_Click(sender, null);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void SetEnableState(bool enabled)
        {
            ProgressBar.IsIndeterminate = !enabled;

            LoginButton.IsEnabled = enabled;

            Token.IsEnabled = enabled;
            NoTokenHyperlink.IsEnabled = enabled;
        }

        #endregion
    }
}
