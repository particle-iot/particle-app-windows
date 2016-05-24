using Particle.Setup;
using Windows.UI.Xaml;

namespace Particle.Tinker.Pages
{
    partial class GetStartedPage
    {
        #region Interaction Methods

        private void GetStartedButton_Click(object sender, RoutedEventArgs e)
        {
            ParticleSetup.Start(TinkerData.SetupConfig, true);
        }

        #endregion

        #region Private Methods

        private void SetupPage()
        {
            var thisPackage = Windows.ApplicationModel.Package.Current;
            var version = thisPackage.Id.Version;
            VersionTextBlock.Text = string.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        #endregion
    }
}
