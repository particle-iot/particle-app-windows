using Particle.SDK;
using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Device
{
    public sealed partial class DeviceInfoPage : UI.GenericPage
    {
        public DeviceInfoPage()
            : base(true)
        {
            InitializeComponent();
            Init(RootGrid);
        }

        protected override void PostNavigatedTo(NavigationEventArgs e)
        {
            particleDevice = (ParticleDevice)e.Parameter;
            SetupPage();
        }
    }
}
