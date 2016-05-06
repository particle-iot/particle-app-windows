using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Auth
{
    public sealed partial class TokenCreatePage : UI.GenericPage
    {
        public TokenCreatePage()
        {
            InitializeComponent();
            Init(RootGrid);
        }

        protected override void PostNavigatedTo(NavigationEventArgs e)
        {
            SetupPage();
        }
    }
}
