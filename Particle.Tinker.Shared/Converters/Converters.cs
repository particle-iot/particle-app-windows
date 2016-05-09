using Particle.SDK;
using Particle.Setup.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Particle.Tinker.Converters
{
    #region Converters

    public class CellularToConnectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return "Cellular";
            else
                return "WiFi";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HideIfContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Visible;
            else if (value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                IDictionary dict = (IDictionary)value;
                return dict.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NameToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var name = (string)value;
            if (string.IsNullOrWhiteSpace(name))
            {
                ResourceLoader resourceLoader = new ResourceLoader();
                return resourceLoader.GetString("UnnamedDevice");
            }
            else
            {
                return name;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class OnlyShowForCoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((ParticleDeviceType)value == ParticleDeviceType.Core)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class OnlyShowForElectronConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((ParticleDeviceType)value == ParticleDeviceType.Electron)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class OnlyShowForPhotonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((ParticleDeviceType)value == ParticleDeviceType.Photon || (ParticleDeviceType)value == ParticleDeviceType.P1)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PlatformIdToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"/Assets/Prototypes/Prototype{value}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StateToColorOrStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((ParticleDeviceState)value)
            {
                case ParticleDeviceState.Tinker:
                    if (targetType == typeof(Brush))
                        return Application.Current.Resources["OnlineDot"];
                    else if (targetType == typeof(string))
                        return "Online";
                    else
                        throw new NotImplementedException();
                case ParticleDeviceState.Online:
                    if (targetType == typeof(Brush))
                        return Application.Current.Resources["OnlineNonTinkerDot"];
                    else if (targetType == typeof(string))
                        return "Online, non-Tinker";
                    else
                        throw new NotImplementedException();
                case ParticleDeviceState.Flashing:
                    if (targetType == typeof(Brush))
                        return Application.Current.Resources["FlashingDot"];
                    else if (targetType == typeof(string))
                        return "Flashing";
                    else
                        throw new NotImplementedException();
                case ParticleDeviceState.Offline:
                    if (targetType == typeof(Brush))
                        return Application.Current.Resources["OfflineDot"];
                    else if (targetType == typeof(string))
                        return "Offline";
                    else
                        throw new NotImplementedException();
                case ParticleDeviceState.Unknown:
                default:
                    if (targetType == typeof(Brush))
                        return Application.Current.Resources["UnknonwDot"];
                    else if (targetType == typeof(string))
                        return "Unknown";
                    else
                        throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StateToEmtyListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((ParticleDeviceState)value)
            {
                case ParticleDeviceState.Tinker:
                case ParticleDeviceState.Online:
                    return "None";
                case ParticleDeviceState.Flashing:
                case ParticleDeviceState.Offline:
                case ParticleDeviceState.Unknown:
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            if (parameter == null)
                return value;

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringKeyValuePairConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var keyValuePair = (KeyValuePair<string, string>)value;
            var stringParameter = (string)parameter;

            if (stringParameter == "key")
                return keyValuePair.Key;
            else if (stringParameter == "value")
                return keyValuePair.Value;
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stringValue = (string)value;
            return stringValue.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
