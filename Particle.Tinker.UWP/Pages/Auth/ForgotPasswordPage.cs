using Particle.SDK;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Particle.Tinker.Pages.Auth
{
    partial class ForgotPasswordPage
    {
        #region Interaction Methods

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private async void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            EmailError.Visibility = Visibility.Collapsed;
            ErrorBorder.Visibility = Visibility.Collapsed;
            SuccessBorder.Visibility = Visibility.Collapsed;

            string errorText = "";
            var email = Email.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorText += resourceLoader.GetString("ErrorEmailIsRequired");
                EmailError.Visibility = Visibility.Visible;
            }

            if (errorText.Length != 0)
            {
                ErrorText.Text = errorText;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            SetEnableState(false);

            var success = await ParticleCloud.SharedCloud.RequestPasswordResetAsync(email);

            if (success)
            {
                SuccessText.Text = resourceLoader.GetString("ForgotPasswordSuccess");
                SuccessBorder.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorText.Text = resourceLoader.GetString("ForgotPasswordError");
                ErrorBorder.Visibility = Visibility.Visible;
            }

            SetEnableState(true);
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                switch (((Control)sender).Name)
                {
                    case "Email":
                        ResetPasswordButton.Focus(FocusState.Pointer);
                        ResetPasswordButton_Click(sender, null);
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

            ResetPasswordButton.IsEnabled = enabled;

            Email.IsEnabled = enabled;
            CancelHyperlink.IsEnabled = enabled;
        }

        #endregion
    }
}
