using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Device
{
    public sealed partial class VariablesPage : UI.GenericPage
    {
        public VariablesPage()
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
