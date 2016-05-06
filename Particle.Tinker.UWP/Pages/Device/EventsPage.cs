using Particle.SDK;
using Particle.SDK.Models;
using System;
using Windows.UI.Xaml;

namespace Particle.Tinker.Pages.Device
{
    partial class EventsPage
    {
        #region Private Members

        ParticleDevice particleDevice = null;
        private Guid? eventListenerID;
        private int eventCount = 0;

        #endregion

        #region Event Handlers

        private void LogEvent(object sender, ParticleEventResponse particeEvent)
        {
            ++eventCount;

            NoEventsProgress.IsActive = false;

            if (eventCount > 100)
                EventsListView.Items.RemoveAt(EventsListView.Items.Count - 1);
                
            EventsListView.Items.Insert(0, particeEvent);
        }

        #endregion

        #region Interaction Methods

        private void PauseAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            UnsubscribeToEvents();
        }

        private void StartAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SubscribeToEvents();
        }

        private void ClearAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            eventCount = 0;
            EventsListView.Items.Clear();
        }

        #endregion

        #region Private Methods

        private void SetupPage()
        {
            DataGrid.DataContext = particleDevice;
            SubscribeToEvents();
        }

        private async void SubscribeToEvents()
        {
            if (eventCount == 0)
                NoEventsProgress.IsActive = true;

            if (eventListenerID == null)
                eventListenerID = await particleDevice.SubscribeToDeviceEventsWithPrefixAsync(LogEvent);
        }

        private void UnsubscribeToEvents()
        {
            if (eventListenerID != null)
            {
                particleDevice.UnsubscribeFromEvent(eventListenerID.Value);
                eventListenerID = null;
            }
        }

        #endregion
    }
}
