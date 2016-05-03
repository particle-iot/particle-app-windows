using Particle.SDK;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Auth
{
    partial class SignupPage
    {
        #region Interaction Methods

        private void HaveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                switch (((Control)sender).Name)
                {
                    case "Email":
                        Password.Focus(FocusState.Keyboard);
                        break;
                    case "Password":
                        PasswordAgain.Focus(FocusState.Keyboard);
                        break;
                    case "PasswordAgain":
                        SignupButton.Focus(FocusState.Pointer);
                        SignupButton_Click(sender, null);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            PasswordAgainError.Visibility = Visibility.Collapsed;
            ErrorBorder.Visibility = Visibility.Collapsed;

            string errorText = "";
            var email = Email.Text;
            var password = Password.Password;
            var passwordAgain = PasswordAgain.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorText += resourceLoader.GetString("ErrorEmailIsRequired");
                EmailError.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                if (errorText.Length != 0)
                    errorText += "\n";
                errorText += resourceLoader.GetString("ErrorPasswordIsRequired");
                PasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(passwordAgain))
                {
                    if (errorText.Length != 0)
                        errorText += "\n";
                    errorText += resourceLoader.GetString("ErrorPasswordsMustMatch");
                    PasswordAgainError.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!password.Equals(passwordAgain))
                    {
                        errorText += resourceLoader.GetString("ErrorPasswordsMustMatch");
                    }
                }
            }

            if (errorText.Length != 0)
            {
                ErrorText.Text = errorText;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            SetEnableState(false);

            var success = await ParticleCloud.SharedCloud.SignupAsync(email, password);

            if (success)
            {
                TinkerData.Login();
                Frame.Navigate(typeof(DevicesPage));
            }
            else
            {
                ErrorText.Text = resourceLoader.GetString("CreateCredentialsError");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }

        #endregion

        #region Private Methods

        private void SetEnableState(bool enabled)
        {
            ProgressBar.IsIndeterminate = !enabled;

            SignupButton.IsEnabled = enabled;

            Email.IsEnabled = enabled;
            Password.IsEnabled = enabled;
            PasswordAgain.IsEnabled = enabled;
            HaveAccountHyperlink.IsEnabled = enabled;
        }

        #endregion
    }
}
