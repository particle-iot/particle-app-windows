using Particle.SDK;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Auth
{
    partial class TokenCreatePage
    {
        #region Interaction Methods

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            ErrorBorder.Visibility = Visibility.Collapsed;

            string errorText = "";
            var email = Email.Text;
            var password = Password.Password;
            int expiresSeconds = 0;
            if (!NeverExpireCheckbox.IsChecked.Value)
            {
                var expireDate = ExpireDate.Date.Date;
                var expireTime = ExpireTime.Time;
                var expiresDateTime = expireDate.Add(expireTime);
                var expires = expiresDateTime - DateTime.Now;
                expiresSeconds = (int)expires.TotalSeconds;
            }

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
            if (!NeverExpireCheckbox.IsChecked.Value)
            {
                if (expiresSeconds < 0)
                {
                    if (errorText.Length != 0)
                        errorText += "\n";
                    errorText += resourceLoader.GetString("TokenCreateExpireTimePast");
                }
                else if (expiresSeconds < 60)
                {
                    if (errorText.Length != 0)
                        errorText += "\n";
                    errorText += resourceLoader.GetString("TokenCreateExpireTimeSoon");
                }
            }

            if (errorText.Length != 0)
            {
                ErrorText.Text = errorText;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            SetEnableState(false);

            var particleAuthentication = await ParticleCloud.SharedCloud.CreateTokenAsync(email, password, expiresSeconds);

            if (particleAuthentication != null)
            {
                NewTokenValue.Text = particleAuthentication.AccessToken;
                NewTokenFlyout.ShowAt(CreateButton);
            }
            else
            {
                ErrorText.Text = resourceLoader.GetString("AuthCredentialsError");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
        
        private void NeverExpireCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (ExpiresGrid != null)
                ExpiresGrid.Visibility = Visibility.Collapsed;
        }

        private void NeverExpireCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ExpiresGrid != null)
                ExpiresGrid.Visibility = Visibility.Visible;
        }

        private void OkButton_click(object sender, RoutedEventArgs e)
        {
            NewTokenFlyout.Hide();
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
                        ExpireDate.Focus(FocusState.Pointer);
                        break;
                    case "ExpireDate":
                        break;
                    case "ExpireTime":
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

            CreateButton.IsEnabled = enabled;

            Email.IsEnabled = enabled;
            Password.IsEnabled = enabled;
            NeverExpireCheckbox.IsEnabled = enabled;
            ExpireDate.IsEnabled = enabled;
            ExpireTime.IsEnabled = enabled;
            CancelHyperlink.IsEnabled = enabled;
        }

        private void SetupPage()
        {
            var expires = DateTime.Now.AddMinutes(60); ;

            ExpireDate.Date = expires;
            ExpireTime.Time = expires.TimeOfDay;
        }

        #endregion
    }
}
