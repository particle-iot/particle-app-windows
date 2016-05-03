using Windows.UI.Xaml.Controls;

namespace Particle.Tinker.Pages.Auth
{
    public sealed partial class TokenLoginPage : UI.GenericPage
    {
        public TokenLoginPage()
        {
            InitializeComponent();
            Init(RootGrid);
        }
    }
}
