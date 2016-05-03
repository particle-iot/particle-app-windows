using Particle.Tinker.Pages.Auth;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages
{
    partial class GetStartedPage
    {
        #region Interaction Methods

        private void GetStartedButton_Click(object sender, RoutedEventArgs e)
        {
            {
                Type authPage = null;
                if (TinkerData.HasSignedIn)
                    authPage = typeof(LoginPage);
                else
                    authPage = typeof(SignupPage);

                Frame.Navigate(authPage);
            }
        }

        #endregion

        #region Private Methods

        private void SetupPage()
        {
            var thisPackage = Windows.ApplicationModel.Package.Current;
            var version = thisPackage.Id.Version;
            VersionTextBlock.Text = string.Format("v{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        #endregion
    }
}
