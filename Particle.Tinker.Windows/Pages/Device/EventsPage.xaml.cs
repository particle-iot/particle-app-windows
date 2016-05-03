using Particle.SDK;
using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages.Device
{
    public sealed partial class EventsPage : UI.GenericPage
    {
        public EventsPage()
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

        protected override void PreNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UnsubscribeToEvents();
        }
    }
}
