using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Particle.UI
{
    public partial class GenericPage : Page
    {
        #region Private Members

        private bool allowHardwareBack = false;

        #endregion

        #region Constructors

        public GenericPage(bool allowHardwareBack = false)
        {
            this.allowHardwareBack = allowHardwareBack;
        }

        #endregion

        #region Public Methods

        public void Init(FrameworkElement root = null)
        {
            if (root != null)
                root.DataContext = VisibleBoundsWindow.VisibleBounds;
        }

        #endregion

        #region Interaction Methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            if (allowHardwareBack)
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif
            PostNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PreNavigatingFrom(e);

#if WINDOWS_PHONE_APP
            if (allowHardwareBack)
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#endif
        }

#if WINDOWS_PHONE_APP
        protected void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = BackPressed();
            if (!e.Handled && Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
#endif

        #endregion

        #region Virtual Methods

        protected virtual void PostNavigatedTo(NavigationEventArgs e)
        {
        }

        protected virtual void PreNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        protected virtual bool BackPressed()
        {
            return false;
        }

        #endregion
    }
}
