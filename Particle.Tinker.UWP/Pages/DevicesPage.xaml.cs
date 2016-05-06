﻿using Windows.UI.Xaml.Navigation;

namespace Particle.Tinker.Pages
{
    public sealed partial class DevicesPage : UI.GenericPage
    {
        public DevicesPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Init(RootGrid);
        }

        protected override async void PostNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            await SetupTinkerAsync();
        }
    }
}
