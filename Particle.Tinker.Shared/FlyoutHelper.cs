using Particle.SDK;
using Particle.SDK.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Particle.Tinker
{
    class FlyoutHelper
    {
        public static void AskReFlashTinker(ParticleDevice particleDevice, Page page, FrameworkElement placementTarget)
        {
            var flyout = (Flyout)page.Resources["ReFlashTinkerFlyout"];
            var actionButton = FindNameInContainer<Button>(flyout.Content, "ReFlashTinkerButton");
            var cancelButton = FindNameInContainer<Button>(flyout.Content, "ReFlashCancelButton");

            actionButton.DataContext = particleDevice;

            RoutedEventHandler actionClickDelegate = null;
            RoutedEventHandler cancelClickDelegate = null;

            actionClickDelegate = new RoutedEventHandler(async delegate (object sender, RoutedEventArgs e) {
                flyout.Hide();
                actionButton.Click -= actionClickDelegate;

                switch (particleDevice.KnownProductId)
                {
                    case ParticleDeviceType.ParticleCore:
                        await particleDevice.FlashKnownAppAsync("tinker");
                        break;
                    case ParticleDeviceType.ParticlePhoton:
                        {
                            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                            StorageFile firmwareFile = await installationFolder.GetFileAsync("Firmware\\PhotonTinker.bin");
                            Stream firmwareStream = await firmwareFile.OpenStreamForReadAsync();
                            await particleDevice.FlashBinaryAsync(firmwareStream, "tinker.bin");
                        }
                        break;
                    case ParticleDeviceType.ParticleP1:
                        {
                            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                            StorageFile firmwareFile = await installationFolder.GetFileAsync("Firmware\\P1Tinker.bin");
                            Stream firmwareStream = await firmwareFile.OpenStreamForReadAsync();
                            await particleDevice.FlashBinaryAsync(firmwareStream, "tinker.bin");
                        }
                        break;
                    case ParticleDeviceType.ParticleElectron:
                        {
                            // TODO: Warn user about data usage
                            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                            StorageFile firmwareFile = await installationFolder.GetFileAsync("Firmware\\ElectronTinker.bin");
                            Stream firmwareStream = await firmwareFile.OpenStreamForReadAsync();
                            await particleDevice.FlashBinaryAsync(firmwareStream, "tinker.bin");
                        }
                        break;
                }
            });

            cancelClickDelegate = new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                flyout.Hide();
                cancelButton.Click -= cancelClickDelegate;
            });

            actionButton.Click += actionClickDelegate;
            cancelButton.Click += cancelClickDelegate;

            flyout.ShowAt(placementTarget);
        }

        public static void AskRenameDevice(ParticleDevice particleDevice, Page page, FrameworkElement placementTarget)
        {
            var flyout = (Flyout)page.Resources["RenameDeviceFlyout"];
            var newDeviceNameGrid = FindNameInContainer<Grid>(flyout.Content, "NewDeviceNameGrid");
            var textbox = FindNameInContainer<TextBox>(flyout.Content, "NewDeviceName");
            var refreshButton = FindNameInContainer<Button>(flyout.Content, "RefreshButton");
            var actionButton = FindNameInContainer<Button>(flyout.Content, "RenameDeviceButton");
            var cancelButton = FindNameInContainer<Button>(flyout.Content, "RenameCancelButton");
            
            textbox.Text = DeviceNameGenerator.GenerateUniqueName(TinkerData.GetDeviceNames());
            textbox.SelectAll();
            
            actionButton.DataContext = particleDevice;

            RoutedEventHandler refreshButtonDelegate = null;
            RoutedEventHandler actionClickDelegate = null;
            RoutedEventHandler cancelClickDelegate = null;
            
            refreshButtonDelegate = new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
            {
                textbox.Text = DeviceNameGenerator.GenerateUniqueName(TinkerData.GetDeviceNames());
                textbox.SelectAll();
            });

            actionClickDelegate = new RoutedEventHandler(async delegate (object sender, RoutedEventArgs e)
            {
                var newName = textbox.Text;

                flyout.Hide();
                refreshButton.Click -= refreshButtonDelegate;
                actionButton.Click -= actionClickDelegate;
                cancelButton.Click -= cancelClickDelegate;

                await particleDevice.RenameAsync(newName);
            });

            cancelClickDelegate = new RoutedEventHandler(delegate (object sender, RoutedEventArgs e)
            {
                flyout.Hide();
                refreshButton.Click -= refreshButtonDelegate;
                actionButton.Click -= actionClickDelegate;
                cancelButton.Click -= cancelClickDelegate;
            });

            refreshButton.Click += refreshButtonDelegate;
            actionButton.Click += actionClickDelegate;
            cancelButton.Click += cancelClickDelegate;

            flyout.ShowAt(placementTarget);
        }

        public static void AskUnclaimDevice(ParticleDevice particleDevice, Page page, FrameworkElement placementTarget)
        {
            var flyout = (Flyout)page.Resources["UnclaimDeviceFlyout"];
            var actionButton = FindNameInContainer<Button>(flyout.Content, "UnclaimDeviceButton");
            var cancelButton = FindNameInContainer<Button>(flyout.Content, "UnclaimCancelButton");

            actionButton.DataContext = particleDevice;

            RoutedEventHandler actionClickDelegate = null;
            RoutedEventHandler cancelClickDelegate = null;

            actionClickDelegate = new RoutedEventHandler(async delegate (object sender, RoutedEventArgs e) {
                actionButton.Click -= actionClickDelegate;

                var unclaimed = await particleDevice.UnclaimAsync();
                if (unclaimed)
                    TinkerData.Devices.Remove(particleDevice);

                flyout.Hide();
            });

            cancelClickDelegate = new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                flyout.Hide();
                cancelButton.Click -= cancelClickDelegate;
            });

            actionButton.Click += actionClickDelegate;
            cancelButton.Click += cancelClickDelegate;

            flyout.ShowAt(placementTarget);
        }

        public static T FindNameInContainer<T>(DependencyObject container, string name)
        {
            T foundChild = default(T);

            int count = VisualTreeHelper.GetChildrenCount(container);
            for (int i = 0; i < count; i++)
            {
                var child = (FrameworkElement)VisualTreeHelper.GetChild(container, i);

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(name))
                {
                    foundChild = (T)(object)child;
                    break;
                }

                foundChild = FindNameInContainer<T>(child, name);

                if (foundChild != null)
                    break;
            }

            return foundChild;
        }

        public static List<T> FindTypeInContainer<T>(DependencyObject container)
        {
            List<T> foundChildren = new List<T>();

            int count = VisualTreeHelper.GetChildrenCount(container);
            for (int i = 0; i < count; i++)
            {
                var child = (FrameworkElement)VisualTreeHelper.GetChild(container, i);

                if (child == null)
                    continue;

                if (child is T)
                    foundChildren.Add((T)(object)child);

                var subFoundChildren = FindTypeInContainer<T>(child);
                    foundChildren.AddRange(subFoundChildren);
            }

            return foundChildren;
        }
    }
}
