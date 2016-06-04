using Particle.SDK;
using Particle.Setup;
using Particle.Tinker.Pages;
using Particle.UI;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker
{
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            var applicationView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            applicationView.VisibleBoundsChanged += ApplicationView_VisibleBoundsChanged;
            applicationView.SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                        transitions.Add(c);
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;
#endif

                ParticleCloud.SharedCloud.SynchronizationContext = System.Threading.SynchronizationContext.Current;
                TinkerData.InitSetup(rootFrame);

                string accessToken = ParticleSetup.AccessToken;
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    ParticleCloud.SharedCloud.SetAuthentication(accessToken);
                    ParticleSetup.Start(TinkerData.SetupConfig, authenticationOnly: true);
                }
                else
                {
                    if (!rootFrame.Navigate(typeof(GetStartedPage), e.Arguments))
                        throw new Exception("Failed to create initial page");
                }
            }
            
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }
#endif

#if WINDOWS_PHONE_APP
        private void ApplicationView_VisibleBoundsChanged(Windows.UI.ViewManagement.ApplicationView applicationView, object e)
        {
            VisibleBoundsWindow.SetBounds(Window.Current.Bounds, applicationView.VisibleBounds);
        }
#endif

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
