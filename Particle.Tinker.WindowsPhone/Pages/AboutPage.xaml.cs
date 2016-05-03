using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages
{
    public sealed partial class AboutPage : UI.GenericPage
    {
        public AboutPage()
            : base(true)
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
