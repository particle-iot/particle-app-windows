using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Device
{
    public sealed partial class FunctionsPage : UI.GenericPage
    {
        public FunctionsPage()
            : base(true)
        {
            InitializeComponent();
            Init(RootGrid);
        }

        protected override void PostNavigatedTo(NavigationEventArgs e)
        {
            startupVariableData = (TinkerPageStartupVariableData)e.Parameter;
            SetupPage();
        }
    }
}
