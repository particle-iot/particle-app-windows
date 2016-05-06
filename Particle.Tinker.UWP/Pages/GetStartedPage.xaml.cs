using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages
{
    public sealed partial class GetStartedPage : UI.GenericPage
    {
        public GetStartedPage()
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
