using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages
{
    partial class AboutPage
    {
        #region Interaction Methods

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        #endregion

        #region Private Methods

        private void SetupPage()
        {
            var thisPackage = Windows.ApplicationModel.Package.Current;
            var version = thisPackage.Id.Version;
            VersionTextBlock.Text = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        #endregion
    }
}
