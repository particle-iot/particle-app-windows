using Particle.SDK;
using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Device
{
    public sealed partial class DevicePinPage : UI.GenericPage
    {
        public DevicePinPage()
            : base(true)
        {
            InitializeComponent();
            Init(RootGrid);
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void PostNavigatedTo(NavigationEventArgs e)
        {
            if (particleDevice != (ParticleDevice)e.Parameter)
            {
                particleDevice = (ParticleDevice)e.Parameter;

                DeviceGrid.DataContext = particleDevice;
                LoadPins();
            }
        }
    }
}
