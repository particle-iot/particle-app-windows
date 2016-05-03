using Particle.SDK;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Auth
{
    partial class LoginPage
    {
        #region Interaction Methods

        private void CreateTokenAppBarButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TokenCreatePage));
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ForgotPasswordPage));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            ErrorBorder.Visibility = Visibility.Collapsed;

            string errorText = "";
            var email = Email.Text;
            var password = Password.Password;

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

            if (errorText.Length != 0)
            {
                ErrorText.Text = errorText;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            SetEnableState(false);

            var success = await ParticleCloud.SharedCloud.LoginAsync(email, password);

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

        private void NeedAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignupPage));
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
                        LoginButton.Focus(FocusState.Pointer);
                        LoginButton_Click(sender, null);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TokenLoginAppBarButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TokenLoginPage));
        }

        #endregion

        #region Private Methods

        private void SetEnableState(bool enabled)
        {
            ProgressBar.IsIndeterminate = !enabled;

            LoginButton.IsEnabled = enabled;

            Email.IsEnabled = enabled;
            Password.IsEnabled = enabled;
            DontHaveAccountHyperlink.IsEnabled = enabled;
            ForgotPasswordHyperlink.IsEnabled = enabled;
        }

        #endregion
    }
}
